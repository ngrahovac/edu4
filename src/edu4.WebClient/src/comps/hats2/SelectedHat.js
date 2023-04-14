import React from 'react'
import AcademicHat from './AcademicHat';
import StudentHat from './StudentHat';

const SelectedHat = (props) => {
    const { hat } = props;
    const hatType = hat.type;

    return (
        <div
            className='my-4 bg-stone-50 px-4 py-6 rounded-md w-full border border-lime-500 bg-lime-50 overflow-clip'>
            {
                (() => {
                    switch (hatType) {
                        case "Student":
                            return <StudentHat hat={hat}></StudentHat>
                        case "Academic":
                            return <AcademicHat hat={hat}></AcademicHat>
                        default:
                            break;
                    }
                })()
            }
        </div>
    )
}

export default SelectedHat