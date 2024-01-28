import React from 'react';
import StudentHat from '../hats2/StudentHat';
import AcademicHat from '../hats2/AcademicHat';
import TertiaryButton from '../buttons/TertiaryButton';

const PositionCardWithApplyOption = (props) => {
    const {
        position,
        onApply = () => {}
    } = props;

    let hat = (
        function () {
            switch (position.requirements.type) {
                case "Student":
                    return <StudentHat hat={position.requirements}></StudentHat>
                case "Academic":
                    return <AcademicHat hat={position.requirements}></AcademicHat>
                default:
                    return;
            }
        }.call(this));

    let borderColor = position.recommended ? "border-lime-300" : "border-indigo-100";

    return (
        <div className={`px-8 py-4 border-4 ${borderColor} flex flex-col gap-y-2`}>
            {hat}

            <p className='font-semibold text-xl text-indigo-500'>{position.name}</p>
            <p className='text-justify text-gray-500 h-max-24 overflow-clip'>{position.description}</p>

            <div className='flex flex-row-reverse'>
                <TertiaryButton onClick={onApply} text="Apply"></TertiaryButton>
            </div>
        </div>
    )
}

export default PositionCardWithApplyOption