import React from 'react'
import AcademicHat from './AcademicHat';
import StudentHat from './StudentHat';

const SelectedHat = (props) => {
    const { hat } = props;
    const hatType = hat.type;

    return (
        <div
            className='px-4 py-4 border-4 border-lime-300'>
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