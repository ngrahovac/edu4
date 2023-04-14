import React from 'react'
import StudentPosition from '../hats2/StudentPosition';
import AcademicPosition from '../hats2/AcademicPosition';
import RecommendedFlair from './RecommendedFlair';

const RecommendedPositionCard = (props) => {
    const {
        position
    } = props;

    return (
        <div className=''>  
            {
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
            }
        </div>
    )
}

export default RecommendedPositionCard