import React from 'react'
import { useState, useEffect } from 'react';
import NeutralButton from '../buttons/NeutralButton';
import AcademicHatForm from './AcademicHatForm';
import StudentHatForm from './StudentHatForm';

const HatForm = (props) => {

    const {
        hatType = "Student",
        onValidChange = () => {},
        onInvalidChange = () => {}
    } = props;

    const [hatTypeState, setHatTypeState] = useState("Student");
    // descendant forms are the ones managing hat state
    // so we're just forwarding handlers and not keeping the state here

    return (
        <>
            <div className='mb-8'>
                <label>
                    <p>Type*</p>
                    <select
                        onChange={e => setHatTypeState(e.target.value)}
                        name="positionType"
                        value={hatTypeState}
                        className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        <option name="Student" value="Student">Student</option>
                        <option name="Academic" value="Academic">Academic</option>
                    </select>
                </label>
            </div>
            {
                (() => {
                    switch (hatTypeState) {
                        case "Student":
                            return (
                                <StudentHatForm
                                    onValidChange={onValidChange}
                                    onInvalidChange={onInvalidChange}>
                                </StudentHatForm>
                            );
                        case "Academic":
                            return (
                                <AcademicHatForm
                                    onValidChange={onValidChange}
                                    onInvalidChange={onInvalidChange}>
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