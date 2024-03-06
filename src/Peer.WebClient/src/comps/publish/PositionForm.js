import React from 'react'
import { useState } from 'react';
import HatForm from '../hat-forms/HatForm';
import InvalidFormFieldWarning from './InvalidFormFieldWarning';

const PositionForm = (props) => {

    const {
        onValidChange,
        onInvalidChange,
        startShowingValidationErrors
    } = props;

    const [position, setPosition] = useState({
        name: '',
        description: '',
        requirements: {}
    });

    const validators = {
        name: name => name.length > 0,
        description: description => description.length > 0,
        requirements: r => true
    };

    function handlePositionFormChange(e) {
        setPosition({ ...position, [e.target.name]: e.target.value });

        let otherPropsValid = Object.keys(position)
            .filter(k => k != e.target.name)
            .map(prop => validators[prop](position[prop]))
            .every(r => r);

        let changedPropValid = validators[e.target.name](e.target.value);

        if (otherPropsValid && changedPropValid) {
            onValidChange({ ...position, [e.target.name]: e.target.value });
        } else {
            onInvalidChange();
        }
    }

    function handleValidHatChange(hat) {
        setPosition({ ...position, requirements: hat });

        let otherPropsValid = Object.keys(position)
            .filter(k => k != "requirements")
            .map(prop => validators[prop](position[prop]))
            .every(r => r);

        if (otherPropsValid) {
            onValidChange({...position, requirements: hat});
        }
    }

    return (
        <>
            <form
                onChange={handlePositionFormChange}>
                <div className='mb-4'>
                    <label>
                    <p className='text-gray-600'>Title*</p>
                        <input
                            type="text"
                            name="name"
                            onChange={handlePositionFormChange}
                            maxLength={100}
                            value={position.name}
                            placeholder='e.g. .NET Backend Developer'
                            className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                        <InvalidFormFieldWarning visible={startShowingValidationErrors && !validators.name(position.name)}></InvalidFormFieldWarning>
                    </label>
                </div>

                <div className='mb-4'>
                    <label>
                    <p className='text-gray-600'>Description*</p>
                        <textarea
                            name="description"
                            onChange={handlePositionFormChange}
                            rows={5}
                            maxLength={1000}
                            value={position.description}
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                        <InvalidFormFieldWarning visible={startShowingValidationErrors && !validators.description(position.description)}></InvalidFormFieldWarning>
                    </label>
                </div>
            </form>

            <HatForm
                onValidChange={handleValidHatChange}
                onInvalidChange={onInvalidChange}>
            </HatForm>
        </>
    )
}

export default PositionForm