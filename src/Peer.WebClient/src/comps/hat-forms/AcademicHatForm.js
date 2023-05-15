import React from 'react'
import { useState, useEffect } from 'react';

const AcademicHatForm = ({ onValidHatChange }) => {

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

    {/* the current form doesn't allow for an invalid hat state */ }
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
                <div className='mb-8'>
                    <label>
                        <p>Research field*</p>
                        <select
                            name="researchField"
                            className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                            {
                                researchFields.map(f =>
                                    <option key={f.id} value={f.value}>{f.value}</option>
                                )
                            }
                        </select>
                    </label>
                </div>
            </div>
        </form>
    )
}

export default AcademicHatForm