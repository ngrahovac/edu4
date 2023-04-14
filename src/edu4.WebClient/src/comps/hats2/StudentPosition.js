import React from 'react'
import StudentHat from './StudentHat'

const StudentPosition = ({ position }) => {
    return (
        <>
            <StudentHat hat={position.requirements}></StudentHat>
            <p className='font-semibold mb-4 mt-2 text-xl'>{position.name}</p>
            <p className='text-justify'>{position.description}</p>
        </>
    )
}

export default StudentPosition