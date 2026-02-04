import React from 'react';

export default function AnchorTable(props) {

	return (
		<div>
			{props?.anchors?.map((anchor, index) => {
				return (
					<div key={index}>
						<div className='anchor-title'>
							<p className='table-item' style={{ paddingLeft: `${anchor.level + 0.5}em` }} onClick={() => props.handleTableClick(anchor)}>
								{props.runs}.{anchor.level}.{0 + index}. {anchor?.content}
							</p>
							<p>
								pg.{anchor.page + 1}
							</p>
						</div>
						{anchor.headerChildren.length > 0 ? (
							<AnchorTable key={index} anchors={anchor.headerChildren} handleTableClick={props.handleTableClick} runs={props.runs} />
						) : undefined}
					</div>
				);
			})}
		</div>
	);
}
