\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- PROCEDUREs for updates to set and element ordering in modules.

-- PROCEDURE: update_element_set(integer, integer)
CREATE OR REPLACE PROCEDURE update_element_set(set_to_move integer, to_order integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
	from_order integer := element_set.order_in_module FROM element_set WHERE element_set.element_set_id = set_to_move;
	set_module integer := element_set.module_id FROM element_set WHERE element_set.element_set_id = set_to_move;
	movement integer := (from_order - to_order) / ABS(from_order - to_order);
	from_range integer := from_order - movement;
	DECLARE item RECORD;
BEGIN
	UPDATE element_set SET order_in_module = -1 WHERE element_set.element_set_id = set_to_move;
	
	FOR item IN 
		SELECT element_set.element_set_id
		FROM element_set
		WHERE element_set.module_id = set_module
			AND element_set.order_in_module BETWEEN LEAST(to_order, from_range) AND GREATEST(to_order, from_range)
		ORDER BY CASE WHEN movement <> -1 THEN element_set.order_in_module * -1 ELSE element_set.order_in_module END ASC
	LOOP
		UPDATE element_set 
			SET order_in_module = order_in_module + movement
			WHERE element_set.element_set_id = item.element_set_id;
	END LOOP;
	
	UPDATE element_set SET order_in_module = to_order WHERE element_set.element_set_id = set_to_move;
END;
$BODY$;

ALTER PROCEDURE update_element_set(set_to_move integer, to_order integer)
    OWNER TO CURRENT_USER;

-- PROCEDURE: update_set_location(integer, integer, integer)
CREATE OR REPLACE PROCEDURE update_set_location(set_to_move integer, element_to_move integer, to_order integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE 
	from_order integer := set_location.order_in_set FROM set_location WHERE set_location.element_set_id = set_to_move AND set_location.element_id = element_to_move;
	movement integer := (from_order - to_order) / ABS(from_order - to_order);
	from_range integer := from_order - movement;
	DECLARE item RECORD;
BEGIN
	UPDATE set_location SET order_in_set = -1 WHERE set_location.element_set_id = set_to_move AND set_location.element_id = element_to_move;
	
	FOR item IN 
		SELECT set_location.element_set_id, set_location.element_id
		FROM set_location
		WHERE set_location.element_set_id = set_to_move
			AND set_location.order_in_set BETWEEN LEAST(to_order, from_range) AND GREATEST(to_order, from_range)
		ORDER BY CASE WHEN movement <> -1 THEN set_location.order_in_set * -1 ELSE set_location.order_in_set END ASC
	LOOP
		UPDATE set_location 
			SET order_in_set = order_in_set + movement
			WHERE set_location.element_set_id = item.element_set_id AND set_location.element_id = item.element_id;
	END LOOP;
	
	UPDATE set_location SET order_in_set = to_order WHERE set_location.element_set_id = set_to_move AND set_location.element_id = element_to_move;

	CALL fix_set_ordering(set_to_move);
END;
$BODY$;

ALTER PROCEDURE update_set_location(set_to_move integer, element_to_move integer, to_order integer)
    OWNER TO CURRENT_USER;

-- PROCEDURE: delete_set_location(integer, integer)
CREATE OR REPLACE PROCEDURE delete_set_location(given_set_id integer, given_element_id integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE 
	place_of_element integer := set_location.order_in_set FROM set_location WHERE set_location.element_set_id = given_set_id AND set_location.element_id = given_element_id;
	DECLARE item RECORD;
BEGIN
	-- Remove item
	DELETE FROM set_location WHERE set_location.element_set_id = given_set_id AND set_location.element_id = given_element_id;
	
	-- Cascade the order_in_set down.
	CALL fix_set_ordering(given_set_id);
END;
$BODY$;

ALTER PROCEDURE delete_set_location(given_set_id integer, given_element_id integer)
    OWNER TO CURRENT_USER;

-- PROCEDURE: delete_set_location(integer, integer)
CREATE OR REPLACE PROCEDURE delete_element_set(given_set_id integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE 
	place_of_set integer := element_set.order_in_module FROM element_set WHERE element_set.element_set_id = given_set_id;
	set_module_id integer := element_set.module_id FROM element_set WHERE element_set.element_set_id = given_set_id;
	DECLARE item RECORD;
BEGIN
	-- Delete all set_locations for the given set.
	DELETE FROM set_location WHERE set_location.element_set_id = given_set_id;
	
	-- Delete the set.
	DELETE FROM element_set WHERE element_set.element_set_id = given_set_id;
	
	-- Cascade the order_in_module down.
	CALL fix_module_ordering(set_module_id);
END;
$BODY$;

ALTER PROCEDURE delete_element_set(given_set_id integer)
    OWNER TO CURRENT_USER;
	
-- PROCEDURE: fix_module_ordering(integer)
CREATE OR REPLACE PROCEDURE fix_module_ordering(given_module_id integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
	DECLARE gap_record RECORD;
	DECLARE to_move RECORD;
BEGIN
	
	WHILE 0 < (SELECT MAX(mo.order_in_module) - (COUNT(mo.order_in_module)-1) as num_gaps
				FROM element_set mo
				WHERE mo.module_id = given_module_id
				GROUP BY mo.module_id)
	LOOP
		SELECT * INTO gap_record
			FROM (SELECT mo.module_id, generate_series(0,MAX(order_in_module)) as order_in_module
					FROM element_set mo
					WHERE mo.module_id = given_module_id
					GROUP BY mo.module_id) AS seq
			WHERE seq.order_in_module NOT IN (SELECT mo.order_in_module
						FROM element_set mo
						WHERE mo.module_id = given_module_id)
			LIMIT 1;
			
		FOR to_move IN SELECT * 
						FROM element_set
						WHERE order_in_module > gap_record.order_in_module
							AND module_id = gap_record.module_id
						ORDER BY order_in_module
		LOOP
			UPDATE element_set
				SET order_in_module = order_in_module-1
				WHERE module_id = to_move.module_id;
		END LOOP;
	END LOOP;
	
END;
$BODY$;

ALTER PROCEDURE fix_module_ordering(given_module_id integer)
    OWNER TO CURRENT_USER;
	
-- PROCEDURE: fix_set_ordering(integer)
CREATE OR REPLACE PROCEDURE fix_set_ordering(given_set_id integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
	DECLARE gap_record RECORD;
	DECLARE to_move RECORD;
BEGIN
	WHILE 0 < (SELECT MAX(mo.order_in_set) - (COUNT(mo.order_in_set)-1) as num_gaps
				FROM set_location mo
				WHERE mo.element_set_id = given_set_id
				GROUP BY mo.element_set_id)
	LOOP
		SELECT * INTO gap_record
			FROM (SELECT mo.element_set_id, generate_series(0,MAX(order_in_set)) as order_in_set
					FROM set_location mo
					WHERE mo.element_set_id = given_set_id
					GROUP BY mo.element_set_id) AS seq
			WHERE seq.order_in_set NOT IN (SELECT mo.order_in_set
						FROM set_location mo
						WHERE mo.element_set_id = given_set_id)
			LIMIT 1;
			
		FOR to_move IN SELECT * 
						FROM set_location
						WHERE order_in_set > gap_record.order_in_set
							AND element_set_id = gap_record.element_set_id
						ORDER BY order_in_set
		LOOP
			UPDATE set_location
				SET order_in_set = order_in_set-1
				WHERE element_set_id = to_move.element_set_id
					AND element_id = to_move.element_id;
		END LOOP;
	END LOOP;
	
END;
$BODY$;

ALTER PROCEDURE fix_set_ordering(given_set_id integer)
    OWNER TO CURRENT_USER;

insert into db_migration (migration_id, description, script_name)
values ('0005', 'create procedures for updates to set and element ordering in modules', '0005.sql');
