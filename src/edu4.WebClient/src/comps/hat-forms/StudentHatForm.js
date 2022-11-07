import React from 'react'
import { useState, useEffect } from 'react';

const StudentHatForm = (props) => {

    const { onValidHatChange } = props;

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

    useEffect(() => {
        onValidHatChange(studentHat);
    }, [studentHat])


    return (
        <form
            id="hat-form"
            onChange={onFormChange}
            onSubmit={e => e.preventDefault()}>
            <div
                className='mb-4 relative'>
                <p
                    className='text-slate-800 text-lg font-semibold'>
                    <span className='superscript'>*</span>
                    Degree
                </p>
                <p
                    className='text-slate-500 text-sm text-justify'>
                    Refers to the study program you're currently enrolled in
                </p>
                <select
                    name="academicDegree"
                    className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                    value={studentHat.parameters.academicDegree}
                    <option value={1}>Bachelor's</option>
                    <option value={2}>Master's</option>
                    <option value={3}>Doctoral</option>
                </select>
            </div>

            <div
                className='mb-4'>
                <p
                    className='text-slate-800 text-lg font-semibold'>
                    <span className='superscript'>*</span>
                    Study field
                </p>
                <select
                    type="text"
                    name="studyField"
                    value={studentHat.parameters.studyField}
                    className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                    {
                        academicFields.map(item => (
                            <option
                                key={item.id}
                                value={item.value}>
                                {item.value}
                            </option>
                        ))
                    }
                </select>
            </div>
        </form>
    )
}

export default StudentHatForm