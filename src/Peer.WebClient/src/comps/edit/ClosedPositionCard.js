import React from 'react'
import StudentPosition from '../hats2/StudentPosition';
import AcademicPosition from '../hats2/AcademicPosition';

const ClosedPositionCard = (props) => {
    const {
        position
    } = props;

    let positionComponent = (
        function () {
            switch (position.requirements.type) {
                case "Student":
                    return <StudentPosition position={position}></StudentPosition>
                case "Academic":
                    return <AcademicPosition position={position}></AcademicPosition>
                default:
                    return;
            }
        }.call(this));

    return (
        <div className='px-8 py-6 border border-gray-300 rounded-lg text-gray-500 bg-gray-100'>
            {positionComponent}
        </div>
    )
}

export default ClosedPositionCard