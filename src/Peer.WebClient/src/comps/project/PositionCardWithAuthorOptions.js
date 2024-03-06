import React from 'react'
import StudentHat from '../hats2/StudentHat';
import AcademicHat from '../hats2/AcademicHat';
import DangerTertiaryButton from '../buttons/DangerTertiaryButton';
import TertiaryButton from '../buttons/TertiaryButton';

const PositionCardWithAuthorOptions = (props) => {
    const {
        position,
        onClose = () => { },
        onReopen = () => { },
        onRemove = () => { }
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

    let borderColor = position.recommended ? "border border-indigo-300" : "border border-gray-200";
    let backgroundColor = position.recommended ? "bg-indigo-50/40" : "bg-white";

    return (
        <div className={`px-8 py-6 rounded-3xl ${borderColor} ${backgroundColor} flex flex-col gap-y-2`}>
            <p className='font-semibold text-xl text-gray-800'>{position.name}</p>
            {hat}
            <p className='text-justify text-gray-600 h-max-24 overflow-clip'>{position.description}</p>
        
            <div className='flex flex-row-reverse'>
                {
                    !position.open &&
                    <TertiaryButton onClick={onReopen} text="Reopen"></TertiaryButton>
                }
                {
                    position.open &&
                    <TertiaryButton onClick={onClose} text="Close"></TertiaryButton>
                }

                <DangerTertiaryButton onClick={onRemove} text="Delete"></DangerTertiaryButton>
            </div>
        </div>
    );
}

export default PositionCardWithAuthorOptions