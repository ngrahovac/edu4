import React, { useEffect } from 'react'
import { useState } from 'react'
import PositionCard from '../discover/PositionCard'
import RecommendedPositionCard from '../discover/RecommendedPositionCard'
import _ from 'lodash'

const ProjectPositions = (props) => {
    const {
        positions,
        onSelectedPositionChanged
    } = props;

    const [selectedPosition, setSelectedPosition] = useState(undefined);

    useEffect(() => {
        if (selectedPosition != undefined) {
            onSelectedPositionChanged(selectedPosition);
        }
    }, [selectedPosition])


    return (
        <div className='flex flex-col space-y-2 mt-4'>
            {
                positions.map(p => <div key={p.id}
                    onClick={() => setSelectedPosition(p)}
                    className={`${_.isEqual(p, selectedPosition) ? "bg-blue-100" : ""} rounded-lg hover:bg-blue-50 cursor-pointer`}>
                    {
                        !p.recommended &&
                        <PositionCard position={p}></PositionCard>
                    }
                    {
                        p.recommended &&
                        <RecommendedPositionCard position={p}></RecommendedPositionCard>
                    }
                </div>)
            }
        </div>
    )
}

export default ProjectPositions