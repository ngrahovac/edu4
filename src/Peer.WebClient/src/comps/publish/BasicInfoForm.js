import React, { useEffect, useRef, useState } from 'react'
import InvalidFormFieldWarning from './InvalidFormFieldWarning';

const BasicInfoForm = (props) => {
    const {
        initialBasicInfo = { title: '', description: '' },
        onValidChange = () => { },
        onInvalidChange = () => { }
    } = props;

    let startShowingValidationErrors = useRef(false);

    const [basicInfo, setBasicInfo] = useState(initialBasicInfo);

    const validators = {
        title: title => title.length > 0,
        description: description => description.length > 0
    };

    function handleBasicInfoFormChange(e) {
        if (!startShowingValidationErrors.current) {
            startShowingValidationErrors.current = true;
        }

        setBasicInfo({ ...basicInfo, [e.target.name]: e.target.value });

        let otherPropsValid = Object.keys(basicInfo)
            .filter(k => k != e.target.name)
            .map(prop => validators[prop](basicInfo[prop]))
            .every(r => r);

        let changedPropValid = validators[e.target.name](e.target.value);

        if (otherPropsValid && changedPropValid) {
            onValidChange({ ...basicInfo, [e.target.name]: e.target.value });
        } else {
            onInvalidChange();
        }
    };

    return (
        <form
            onChange={handleBasicInfoFormChange}>
            <div>
                <label>
                    <p>Title*</p>
                    <input
                        type="text"
                        name="title"
                        onChange={handleBasicInfoFormChange}
                        maxLength={100}
                        value={basicInfo.title}
                        className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                    <InvalidFormFieldWarning visible={startShowingValidationErrors.current && !validators.title(basicInfo.title)}></InvalidFormFieldWarning>
                </label>
            </div>

            <div>
                <label>
                    <p>Description*</p>
                    <textarea
                        rows={5}
                        maxLength={1000}
                        name="description"
                        onChange={handleBasicInfoFormChange}
                        value={basicInfo.description}
                        className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                    <InvalidFormFieldWarning visible={startShowingValidationErrors.current && !validators.description(basicInfo.description)}></InvalidFormFieldWarning>
                </label>
            </div>
        </form>
    )
}

export default BasicInfoForm