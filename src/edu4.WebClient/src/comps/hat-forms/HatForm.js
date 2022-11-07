import React from 'react'
import { useState, useEffect } from 'react';
import AcademicHatForm from './AcademicHatForm';
import StudentHatForm from './StudentHatForm';

const HatForm = (props) => {

    const {
        hatType,
        onHatAdded
    } = props;

    const [hat, setHat] = useState(null);

    const [canAddHat, setCanAddHat] = useState(false);

    useEffect(() => {
        setCanAddHat(hat != null);
    }, [hat])


    return (
        <div
            className='relative pb-32'>
            {
                (() => {
                    switch (hatType) {
                        case "Student":
                            return (
                                <StudentHatForm
                                    onValidHatChange={(hat) => { setHat(hat) }}>
                                </StudentHatForm>
                            );
                        case "Academic":
                            return (
                                <AcademicHatForm
                                    onValidHatChange={(hat) => { setHat(hat) }}>
                                </AcademicHatForm>
                            );
                        default:
                            break;
                    }
                })()
            }

            <button
                type='button'
                onClick={() => {
                    if (canAddHat)
                        onHatAdded(hat)
                }}
                className='absolute bottom-16 right-0 px-4 py-2 rounded-md bg-stone-400 hover:bg-stone-500 text-stone-50 font-semibold text-lg'>
                Add
            </button>
        </div>
    )
}

export default HatForm