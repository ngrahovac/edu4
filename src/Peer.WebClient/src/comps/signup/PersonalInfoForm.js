import React, { useEffect, useState } from 'react'

const PersonalInfoForm = (props) => {

    const {
        onValidChange = () => { },
        onInvalidChange = () => { },
        user
    } = props;

    const [personalInfo, setPersonalInfo] = useState({ fullName: user.name, contactEmail: user.email });

    const [propValidity, setPropValidity] = useState({
        fullName: true,
        contactEmail: true
    });

    useEffect(() => {
        Object.values(propValidity).every(v => v) ?
            onValidChange(personalInfo) :
            onInvalidChange();
    }, [propValidity, personalInfo]);

    function onPersonalInfoFormChange(e) {
        setPersonalInfo({ ...personalInfo, [e.target.name]: e.target.value });
    };

    function validate() {
        setPropValidity({
            fullName: personalInfo.fullName.length > 0,
            contactEmail: personalInfo.contactEmail.length > 0
        });
    }

    return (
        <form
            onChange={onPersonalInfoFormChange}
            onBlur={validate}>
            <div className='mb-4'>
                <label>
                    <p>Full name*</p>
                    <input
                        type="text"
                        name="fullName"
                        value={personalInfo.fullName}
                        className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                    <p className='text-red-500 font-semibold h-8'>{`${propValidity.fullName ? "" : "The provided value is not valid"}`}</p>
                </label>
            </div>

            <div className='mb-4'>
                <label>
                    <p>Contact email*</p>
                    <input
                        type="text"
                        name="contactEmail"
                        value={personalInfo.contactEmail}
                        className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                    </input>
                    <p className='text-red-500 font-semibold h-8'>{`${propValidity.contactEmail ? "" : "The provided value is not valid"}`}</p>
                </label>
            </div>
        </form>
    )
}

export default PersonalInfoForm