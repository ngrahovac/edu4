import React from 'react'
import StudentPosition from '../hats2/StudentPosition';
import AcademicPosition from '../hats2/AcademicPosition';

const PositionCard = (props) => {
    const {
        position
    } = props;

    return (
        function () {
            switch (position.requirements.type) {
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

export default PositionCard