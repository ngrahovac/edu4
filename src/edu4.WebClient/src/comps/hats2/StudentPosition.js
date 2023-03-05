import React from 'react'
import StudentHat from './StudentHat'

const StudentPosition = ({ position }) => {
    return (
        <div className='px-8 py-6 border border-gray-300 rounded-lg'>
            <StudentHat hat={position.hat}></StudentHat>
            <p className='font-bold mb-4'>{position.title}</p>
            <p className='text-justify'>{position.description}</p>
        </div>
    )
}

export default StudentPosition