import React from 'react'
import AcademicHat from '../hats2/AcademicHat';
import StudentHat from '../hats2/StudentHat';

const Hat = (props) => {

    const { hat } = props;
    const hatType = hat.type;

    return (
        <div
            className='my-4 px-4 py-6 rounded-md w-full border border-gray-300 overflow-clip'>
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

export default Hat