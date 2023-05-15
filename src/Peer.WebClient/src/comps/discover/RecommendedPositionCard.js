import React from 'react'
import StudentPosition from '../hats2/StudentPosition';
import AcademicPosition from '../hats2/AcademicPosition';
import RecommendedFlair from './RecommendedFlair';

const RecommendedPositionCard = (props) => {
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
        <div className='relative'>
            <div className=' px-8 py-6 border border-lime-300 rounded-lg bg-lime-50/10'>
                {positionComponent}
            </div>
            
            <div className='absolute top-0 right-0'>
                <RecommendedFlair></RecommendedFlair>
            </div>
        </div>
    )
}

export default RecommendedPositionCard