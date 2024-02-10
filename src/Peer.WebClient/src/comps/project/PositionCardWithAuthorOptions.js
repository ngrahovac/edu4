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

    let borderColor = position.open ? "border-indigo-100" : "border-gray-300";

    return (
        <div className={`px-8 py-4 border-4 ${borderColor} flex flex-col gap-y-2`}>
            {hat}

            <p className='font-semibold text-xl text-indigo-500'>{position.name}</p>
            <p className='text-justify text-gray-500 h-max-24 overflow-clip'>{position.description}</p>

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
    )
}

export default PositionCardWithAuthorOptions