import React from 'react'
import StudentHat from './StudentHat'

const StudentPosition = ({ position }) => {
    return (
        <div className='px-8 py-6 border border-gray-300 rounded-lg'>
            <StudentHat hat={position.requirements}></StudentHat>
            <p className='font-semibold mb-4 mt-2 text-xl'>{position.name}</p>
            <p className='text-justify'>{position.description}</p>
        </div>
    )
}

export default StudentPosition