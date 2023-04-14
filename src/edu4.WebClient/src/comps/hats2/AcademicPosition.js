import React from 'react'
import AcademicHat from './AcademicHat'

const AcademicPosition = ({ position }) => {
    return (
        <div className='px-8 py-6 border border-gray-300 rounded-lg'>
            <AcademicHat hat={position.requirements}></AcademicHat>
            <p className='font-semibold'>{position.title}</p>
            <p className='font-semibold mb-4 text-xl mt-2'>{position.name}</p>
            <p className='text-justify'>{position.description}</p>
        </div>
    )
}

export default AcademicPosition