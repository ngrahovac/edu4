import React from 'react'
import StudentPosition from './StudentPosition'
import AcademicPosition from './AcademicPosition'

const Position = ({ position }) => {

    return (
        function () {
            switch (position.hat.type) {
                case "Student":
                    return <StudentPosition position={position}></StudentPosition>
                case "Academic":
                    return <AcademicPosition position={position}></AcademicPosition>
                default:
                    return;
            }
        }.call(this)
    )
}

export default Position