import React from 'react'
import { useState, useEffect } from 'react';

const StudentHatForm = ({ onValidHatChange }) => {

    const [studentHat, setStudentHat] = useState({
        type: "Student",
        parameters: {
            academicDegree: 1,
            studyField: "Computer Science"
        }
    })

    /* TODO: fetch from the API */
    const academicFields = [
        { id: 1, value: "Computer Science" },
        { id: 2, value: "Mechanical Engineering" },
        { id: 3, value: "Electronics Engineering" },
        { id: 4, value: "Economics" }
    ]

    function onFormChange(e) {
        if (e.target.name == "academicDegree") {
            setStudentHat({
                ...studentHat,
                parameters: {
                    ...studentHat.parameters,
                    academicDegree: parseInt(e.target.value)
                }
            });
            return;
        }

        setStudentHat({
            ...studentHat,
            parameters: {
                ...studentHat.parameters,
                [e.target.name]: e.target.value
            }
        })
    }

    {/* the current form doesn't allow for an invalid hat state */ }
    useEffect(() => {
        onValidHatChange(studentHat);
    }, [studentHat])

    return (
        <form
            id="hat-form"
            onChange={onFormChange}
            onSubmit={e => e.preventDefault()}>

            <div className='mb-8'>
                <label>
                    <p>Study field*</p>
                    <select
                        name="studyField"
                        className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        {
                            academicFields.map(f =>
                                <option key={f.id} value={f.value}>{f.value}</option>
                            )
                        }
                    </select>
                </label>
            </div>

            <div className='mb-16'>
                <label>
                    <p>Degree*</p>
                    <select
                        name="academicDegree"
                        className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        <option value="1">BSc</option>
                        <option value="2">MSc</option>
                        <option value="3">PhD</option>
                    </select>
                </label>
            </div>
        </form>
    )
}

export default StudentHatForm