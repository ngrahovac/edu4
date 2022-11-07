import React from 'react'
import { useState, useEffect } from 'react';

const AcademicHatForm = (props) => {

    const { onValidHatChange } = props;

    const [academicHat, setAcademicHat] = useState({
        type: "Academic",
        parameters: {
            researchField: "Computer Science"
        }
    })

    /* TODO: fetch from the API */
    const researchFields = [
        { id: 1, value: "Computer Science" },
        { id: 2, value: "Mechanical Engineering" },
        { id: 3, value: "Electronics Engineering" },
        { id: 4, value: "Economics" }
    ]

    function onFormChange(e) {
        setAcademicHat({
            ...academicHat,
            parameters: {
                ...academicHat.parameters,
                [e.target.name]: e.target.value
            }
        })
    }

    useEffect(() => {
        onValidHatChange(academicHat);
    }, [academicHat])


    return (
        <form
            id="hat-form"
            onChange={onFormChange}
            onSubmit={e => e.preventDefault()}>
            <div
                className='mb-4'>
                <p
                    className='text-slate-800 text-lg font-semibold'>
                    <span className='superscript'>*</span>
                    Research field
                </p>
                <select
                    type="text"
                    name="researchField"
                    value={academicHat.parameters.researchField}
                    className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                    {
                        researchFields.map(item => (
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

export default AcademicHatForm