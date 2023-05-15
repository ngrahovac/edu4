import React from 'react'
import { useState, useEffect } from 'react';
import NeutralButton from '../buttons/NeutralButton';
import AcademicHatForm from './AcademicHatForm';
import StudentHatForm from './StudentHatForm';

const HatForm = (props) => {

    const {
        hatType,
        onHatChanged
    } = props;

    const [hat, setHat] = useState(null);

    useEffect(() => {
        onHatChanged(hat);
    }, [hat])

    return (
        <>
            {
                (() => {
                    switch (hatType) {
                        case "Student":
                            return (
                                <StudentHatForm
                                    onValidHatChange={setHat}>
                                </StudentHatForm>
                            );
                        case "Academic":
                            return (
                                <AcademicHatForm
                                    onValidHatChange={setHat}>
                                </AcademicHatForm>
                            );
                        default:
                            break;
                    }
                })()
            }
        </>
    )
}

export default HatForm