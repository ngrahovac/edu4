import React from 'react'
import Position from './Position'

const AddedPosition = (props) => {
    const {
        position,
        onRemoved
    } = props;

    return (
        <div className='relative'>
            <div
                onClick={onRemoved}
                className='absolute top-2 right-2'>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 hover:stroke-red-700">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                </svg>
            </div>

            <Position position={position}>
            </Position>
        </div>
    )
}

export default AddedPosition