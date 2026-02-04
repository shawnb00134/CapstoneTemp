\echo Applying 0002 privilege materialized views to schema cam_cms
SET search_path TO cam_cms, public;

-- PROCEDURE: fix_root_folder_ordering(integer)

-- DROP PROCEDURE IF EXISTS fix_root_folder_ordering(integer);

CREATE OR REPLACE PROCEDURE fix_root_folder_ordering(
	IN param_package_id integer)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    current_order RECORD;
    new_order INT := 0;
BEGIN
    FOR current_order IN
        SELECT * FROM package_folder AS folder
        WHERE folder.package_id = PARAM_PACKAGE_ID AND
              folder.is_deleted = false AND
              folder.parent_folder_id IS NULL
        ORDER BY order_in_parent ASC
    LOOP
        -- Update the order_in_parent column
        UPDATE package_folder
        SET order_in_parent = new_order
		WHERE package_folder.package_folder_id = current_order.package_folder_id;

        -- Output a notice with the old and new order values
        RAISE NOTICE 'Order_in_parent: % is now %', current_order.order_in_parent, new_order;

        -- Increment the new order
        new_order := new_order + 1;
    END LOOP;
END;
$BODY$;
ALTER PROCEDURE fix_root_folder_ordering(integer)
    OWNER TO CURRENT_USER;


-- PROCEDURE: update_package_folder_module(integer, integer, integer)
CREATE OR REPLACE PROCEDURE update_package_folder_module(package_to_move integer, module_to_move integer, to_order integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE 
	from_order integer := package_folder_module.order_in_folder FROM package_folder_module WHERE package_folder_module.package_folder_id = package_to_move AND package_folder_module.package_folder_module_id = module_to_move;
	movement integer := (from_order - to_order) / ABS(from_order - to_order);
	from_range integer := from_order - movement;
	DECLARE item RECORD;
BEGIN
	-- Update the item to be moved to -1
	UPDATE package_folder_module SET order_in_folder = -1 WHERE package_folder_module.package_folder_id = package_to_move AND package_folder_module.package_folder_module_id = module_to_move;
	
	-- Update all items between the from and to ranges
	FOR item IN 
		SELECT package_folder_module.package_folder_id, package_folder_module.package_folder_module_id 
		FROM package_folder_module
		WHERE package_folder_module.package_folder_id = package_to_move
			AND package_folder_module.order_in_folder  BETWEEN LEAST(to_order, from_range) AND GREATEST(to_order, from_range)
		ORDER BY CASE WHEN movement <> -1 THEN package_folder_module.order_in_folder  * -1 ELSE package_folder_module.order_in_folder  END ASC
	LOOP
		UPDATE package_folder_module 
			SET order_in_folder  =order_in_folder  + movement
			WHERE package_folder_module.package_folder_id = item.package_folder_id AND package_folder_module.package_folder_module_id = item.package_folder_module_id;
	END LOOP;
	
	-- Update the item to be moved to the new location
	UPDATE package_folder_module SET order_in_folder = to_order WHERE package_folder_module.package_folder_id = package_to_move AND package_folder_module.package_folder_module_id = module_to_move;

	-- Fix the ordering of the package_folder
	CALL fix_package_folder_ordering(package_to_move);
END;
$BODY$;

ALTER PROCEDURE update_package_folder_module(package_to_move integer, module_to_move integer, to_order integer)
    OWNER TO CURRENT_USER;

-- PROCEDURE: update_package_folder(integer, integer)
CREATE OR REPLACE PROCEDURE update_package_folder(folder_to_move integer, to_order integer)
    LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    from_order integer := package_folder.order_in_parent FROM package_folder WHERE package_folder.package_folder_id = folder_to_move;
    folder_parent integer := package_folder.parent_folder_id FROM package_folder WHERE package_folder.package_folder_id = folder_to_move;
    folder_package integer := package_folder.package_id FROM package_folder WHERE package_folder.package_folder_id = folder_to_move;
    movement integer := (from_order - to_order) / ABS(from_order - to_order);
    from_range integer := from_order - movement;
    DECLARE item RECORD;
BEGIN
	-- Update the item to be moved to -1
    UPDATE package_folder SET order_in_parent = -1 WHERE package_folder.package_folder_id = folder_to_move;

	-- Update all items between the from and to ranges
    FOR item IN 
        SELECT package_folder.package_folder_id
        FROM package_folder
        WHERE package_folder.package_id = folder_package
            AND (package_folder.parent_folder_id IS NULL OR package_folder.parent_folder_id = folder_parent)
            AND package_folder.order_in_parent BETWEEN LEAST(to_order, from_range) AND GREATEST(to_order, from_range)
        ORDER BY CASE WHEN movement <> -1 THEN package_folder.order_in_parent * -1 
                ELSE package_folder.order_in_parent END ASC
    LOOP
        UPDATE package_folder 
            SET order_in_parent = order_in_parent + movement
            WHERE package_folder.package_folder_id = item.package_folder_id;
    END LOOP;

	-- Update the item to be moved to the new location
    UPDATE package_folder SET order_in_parent = to_order WHERE package_folder.package_folder_id = folder_to_move;

	-- Fix the ordering of the package folder
	IF folder_parent IS NULL THEN
		CALL fix_package_ordering(folder_package);
	ELSE
		CALL fix_package_folder_ordering(folder_parent);
	END IF;
END;
$BODY$;

ALTER PROCEDURE update_package_folder(folder_to_move integer, to_order integer)
	OWNER TO CURRENT_USER;

-- PROCEDURE: fix_package_folder_ordering(integer)
CREATE OR REPLACE PROCEDURE fix_package_folder_ordering(given_package_folder_id integer)
	LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
	DECLARE item RECORD;
BEGIN	
	-- Gather valid orderings for package_folder_id
	-- FIXME: Needs permission to create temp table
	-- CREATE TEMPORARY TABLE wanted_orderings AS
	-- SELECT generate_series(0,COUNT(folder_orderings.order)-1) as true_order
	-- FROM (SELECT package_folder.package_folder_id AS id, package_folder.order_in_parent AS order
	-- 		FROM package_folder
	-- 		WHERE package_folder.parent_folder_id = given_package_folder_id
	-- 	UNION All
	-- 		SELECT package_folder_module.package_folder_module_id AS id, package_folder_module.order_in_folder AS order
	-- 		FROM package_folder_module) as folder_orderings;

	-- Gather current orderings for package_folder_id (modules and sub folders)
	-- FIXME: Needs permission to create temp table
	-- CREATE TEMPORARY TABLE actual_orderings AS
	-- SELECT *
	-- FROM (SELECT package_folder.package_folder_id AS id, package_folder.order_in_parent AS order
	-- 		FROM package_folder
	-- 		WHERE package_folder.parent_folder_id = given_package_folder_id
	-- 	UNION All
	-- 		SELECT package_folder_module.package_folder_module_id AS id, package_folder_module.order_in_folder AS order, FALSE AS is_package_folder
	-- 		FROM package_folder_module
	-- 		WHERE package_folder_module.package_folder_id = given_package_folder_id) AS folder_orderings
	-- ORDER BY folder_orderings.order ASC;

	-- Iterate through each item and update its position
	FOR item IN SELECT *
				FROM (
					WITH numbered_wanted_orderings AS (
						SELECT *, ROW_NUMBER() OVER (ORDER BY true_orderings.true_order) AS row_number
						FROM (SELECT generate_series(0,COUNT(folder_orderings.order)-1) as true_order
								FROM (SELECT package_folder.package_folder_id AS id, package_folder.order_in_parent AS order
										FROM package_folder
										WHERE package_folder.parent_folder_id = given_package_folder_id
									UNION ALL
										SELECT package_folder_module.package_folder_module_id AS id, package_folder_module.order_in_folder AS order
										FROM package_folder_module) as folder_orderings) AS true_orderings
						),
						numbered_actual_orderings AS (
						SELECT *, ROW_NUMBER() OVER (ORDER BY ordered_folder_orderings.order) AS row_number
						FROM (SELECT *
								FROM (SELECT package_folder.package_folder_id AS id, package_folder.order_in_parent AS order, TRUE AS is_package_folder
										FROM package_folder
										WHERE package_folder.parent_folder_id = given_package_folder_id
									UNION All
										SELECT package_folder_module.package_folder_module_id AS id, package_folder_module.order_in_folder AS order, FALSE AS is_package_folder
										FROM package_folder_module
										WHERE package_folder_module.package_folder_id = given_package_folder_id) AS folder_orderings
								ORDER BY folder_orderings.order ASC) AS ordered_folder_orderings
						)
					SELECT numbered_wanted_orderings.true_order, numbered_actual_orderings.order, numbered_actual_orderings.id, numbered_actual_orderings.is_package_folder
					FROM numbered_wanted_orderings
						JOIN numbered_actual_orderings ON numbered_wanted_orderings.row_number = numbered_actual_orderings.row_number
				) AS to_update
				ORDER BY to_update.true_order ASC
	LOOP
		IF item.is_package_folder
		THEN
			UPDATE package_folder
			SET order_in_parent=item.true_order
			WHERE package_folder.package_folder_id=item.id;
		ELSE
			UPDATE package_folder_module
			SET order_in_folder=item.true_order
			WHERE package_folder_module.package_folder_module_id=item.id;
		END IF;
	END LOOP;
END;
$BODY$;

ALTER PROCEDURE fix_package_folder_ordering(given_package_id integer)
	OWNER TO CURRENT_USER;

-- Procedure: fix_package_ordering(integer)
CREATE OR REPLACE PROCEDURE fix_package_ordering(given_package_id integer)
	LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
	DECLARE item RECORD;
BEGIN	
	-- Gather valid orderings for package_folder_id
	-- 	CREATE TEMPORARY TABLE wanted_orderings AS
	-- 	SELECT generate_series(0,COUNT(package_folder.package_id)-1) as true_order
	-- 	FROM package_folder
	-- 	WHERE package_folder.package_id = package_id
	-- 		AND package_folder.parent_folder_id IS NULL;

	-- Gather current orderings for package_folder_id (modules and sub folders)
	-- 	CREATE TEMPORARY TABLE actual_orderings AS
	-- 	SELECT *
	-- 	FROM package_folder
	-- 	WHERE package_folder.package_id = package_id
	-- 		AND package_folder.parent_folder_id IS NULL
	-- 	ORDER BY package_folder.order_in_parent ASC;

	-- Map correct orderings to current orderings
	-- 	CREATE TEMPORARY TABLE to_update AS
	-- 	WITH numbered_wanted_orderings AS (
	-- 		  SELECT *, ROW_NUMBER() OVER (ORDER BY true_order) AS row_number
	-- 		  FROM wanted_orderings
	-- 		),
	-- 		numbered_actual_orderings AS (
	-- 		  SELECT *, ROW_NUMBER() OVER (ORDER BY order_in_parent) AS row_number
	-- 		  FROM actual_orderings
	-- 		)
	-- 	SELECT wanted_orderings.true_order, actual_orderings.*
	-- 	FROM numbered_wanted_orderings wanted_orderings
	-- 		JOIN numbered_actual_orderings actual_orderings ON wanted_orderings.row_number = actual_orderings.row_number;

	-- Iterate through each item and update its position
	FOR item in (
		SELECT * 
		FROM (
			WITH numbered_wanted_orderings AS (
			  SELECT *, ROW_NUMBER() OVER (ORDER BY wanted_orderings.true_order) AS row_number
			  FROM (
			  	SELECT generate_series(0,COUNT(package_folder.package_id)-1) as true_order
				FROM package_folder
				WHERE package_folder.package_id = given_package_id
					AND package_folder.parent_folder_id IS NULL
			  ) AS wanted_orderings
			),
			numbered_actual_orderings AS (
			  SELECT *, ROW_NUMBER() OVER (ORDER BY actual_orderings.order_in_parent) AS row_number
			  FROM (
			  	SELECT *
				FROM package_folder
				WHERE package_folder.package_id = given_package_id
					AND package_folder.parent_folder_id IS NULL
				ORDER BY package_folder.order_in_parent ASC
			  ) AS actual_orderings
			)
		SELECT wanted_orderings.true_order, actual_orderings.*
		FROM numbered_wanted_orderings wanted_orderings
			JOIN numbered_actual_orderings actual_orderings ON wanted_orderings.row_number = actual_orderings.row_number
		) AS to_update 
		ORDER BY true_order ASC
	)
	LOOP
		UPDATE package_folder
		SET order_in_parent = item.true_order
		WHERE package_folder.package_folder_id = item.package_folder_id;
	END LOOP;
END;
$BODY$;

ALTER PROCEDURE fix_package_ordering(package_id integer)
	OWNER TO CURRENT_USER;

insert into db_migration (migration_id, description, script_name)
values ('0007', 'create procedures for package ordering', '0007.sql');