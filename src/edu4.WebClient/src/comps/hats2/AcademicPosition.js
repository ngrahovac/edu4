import React from 'react'
import AcademicHat from './AcademicHat'

const AcademicPosition = ({ position }) => {
    return (
        <div className='px-8 py-6 border border-gray-300 rounded-lg'>
            <AcademicHat hat={position.hat}></AcademicHat>
            <p className='font-bold mb-4'>{position.title}</p>
            <p className='text-justify'>{position.description}</p>
        </div>
    )
}

export default AcademicPosition