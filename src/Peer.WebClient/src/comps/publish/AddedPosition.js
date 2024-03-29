import React from 'react'
import PositionCard from '../discover/PositionCard';

const AddedPosition = (props) => {
    const {
        position,
        onRemoved
    } = props;

    return (
        <div className='relative'>
            <div
                onClick={() => onRemoved(position)}
                className='absolute top-4 right-4'>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="gray" className="w-5 h-5 hover:stroke-red-700">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                </svg>
            </div>

            <PositionCard position={position}></PositionCard>
        </div>
    )
}

export default AddedPosition