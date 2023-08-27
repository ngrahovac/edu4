import React from 'react'
import { useEffect, useState } from 'react';
import HatForm from '../hat-forms/HatForm';

const PositionForm = (props) => {

    const {
        onValidChange,
        onInvalidChange
    } = props;

    const [positionState, setPositionState] = useState({
        name: '',
        description: '',
        requirements: {}
    });

    const [propValidity, setPropValidity] = useState({
        name: false,
        description: false,
        requirements: false
    });

    function validate() {
        setPropValidity({
            name: positionState.name.length > 0,
            description: positionState.description.length > 0,
            requirements: true
        });
    }

    useEffect(() => {
        Object.values(propValidity).every(v => v) ?
            onValidChange(positionState) :
            onInvalidChange();
    }, [propValidity, positionState]);

    function onPositionFormChange(e) {
        setPositionState({ ...positionState, [e.target.name]: e.target.value });
    }

    return (
        <>
            <form
                onChange={onPositionFormChange}
                onBlur={validate}>
                <div className='mb-4'>
                    <label>
                        <p>Title*</p>
                        <input
                            type="text"
                            name="name"
                            value={positionState.name}
                            placeholder='e.g. .NET Backend Developer'
                            className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                        <p className='text-red-500 font-semibold h-8'>{`${propValidity.name ? "" : "The provided value is not valid"}`}</p>
                    </label>
                </div>

                <div className='mb-4'>
                    <label>
                        <p>Description*</p>
                        <textarea
                            name="description"
                            rows={5}
                            maxLength={1000}
                            value={positionState.description}
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                        <p className='text-red-500 font-semibold h-8'>{`${propValidity.description ? "" : "The provided value is not valid"}`}</p>

                    </label>
                </div>
            </form>

            <HatForm
                onValidChange={hat => setPositionState({ ...positionState, requirements: hat })}
                onInvalidChange={onInvalidChange}>
            </HatForm>
        </>
    )
}

export default PositionForm