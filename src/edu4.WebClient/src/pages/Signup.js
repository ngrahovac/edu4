import React from 'react'
import { useState, useEffect } from 'react'
import VisibilityFlair from '../comps/accents/VisibilityFlair'
import HatForm from '../comps/hat-forms/HatForm'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import Hat from '../comps/hats/Hat'
import { signUp } from '../services/SignupService'
import { useAuth0 } from '@auth0/auth0-react'
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import UserActionSuccessToast from '../comps/toasts/UserActionSuccessToast'
import UserActionFailToast from '../comps/toasts/UserActionFailToast'

const Signup = () => {
    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const [signupModel, setSignupModel] = useState({
        contactEmail: "",
        fullName: "",
        hats: []
    })

    const [selectedHatType, setSelectedHatType] = useState("Student")

    const [validation, setValidation] = useState({
        hats: {
            validator: h => h.length > 0,
            valid: true,
            errorMessage: "Make sure to add at least one hat"
        }
    })

    function validateField(fieldName) {
        if (Object.hasOwn(validation, fieldName)) {
            let validator = validation[fieldName].validator;
            let validationResult = validator(signupModel[fieldName]);

            setValidation({
                ...validation,
                [fieldName]: {
                    ...validation[fieldName],
                    valid: validationResult
                }
            })
        }
    }

    useEffect(() => {
        validateField("hats")
    }, [signupModel])

    function addHat(hat) {
        setSignupModel({
            ...signupModel,
            hats: [...signupModel.hats, hat]
        })
    }

    function removeHat(hatToRemove) {
        var filteredHats = signupModel.hats.filter(h => h !== hatToRemove);

        setSignupModel({
            ...signupModel,
            hats: filteredHats
        });
    }

    function onFormChange(e) {
        if (e.target.name === "fullName" || e.target.name === "contactEmail") {
            setSignupModel({ ...signupModel, [e.target.name]: e.target.value })
            return;
        }

        if (e.target.name === "hatType") {
            setSelectedHatType(e.target.value);
        }
    }

    function onSignupRequested() {
        (async () => {
            try {
                {/* validation of other parts of the model is handled on HTML level */ }
                if (signupModel.hats.length > 0) {
                    let token = await getAccessTokenWithPopup({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await signUp(signupModel, token);

                    if (result.outcome === successResult) {
                        document.getElementById('user-action-success-toast').show();
                        setTimeout(() => window.location.href = "/homepage", 1000);
                    } else if (result.outcome === failureResult) {
                        document.getElementById('user-action-fail-toast').show();
                        setTimeout(() => {
                            document.getElementById('user-action-fail-toast').close();
                        }, 3000);
                    } else if (result.outcome === errorResult) {
                        document.getElementById('user-action-fail-toast').show();
                        setTimeout(() => {
                            document.getElementById('user-action-fail-toast').close();
                        }, 3000);
                    }
                }
            } catch (ex) {
                document.getElementById('user-action-fail-toast').show();
                setTimeout(() => {
                    document.getElementById('user-action-fail-toast').close();
                }, 3000);
            }
        })();
    }

    return (
        <>
            <UserActionSuccessToast
                caption="Signup successfully completed!">
            </UserActionSuccessToast>

            <UserActionFailToast
                caption="An error occurred. Please try again later.">
            </UserActionFailToast>

            <SingleColumnLayout
                title="Tell us about yourself">
                <>
                    <form
                        id="parent-form"
                        onChange={onFormChange}
                        onSubmit={e => { e.preventDefault(); onSignupRequested(); }}>
                        <div
                            className='mb-4'>
                            <p
                                className='text-slate-800 text-lg font-semibold'>
                                <span className='superscript'>*</span>
                                Full name
                                <VisibilityFlair text="public"></VisibilityFlair>
                            </p>
                            <input
                                type="text"
                                name="fullName"
                                value={signupModel.fullName}
                                required
                                minLength={2}
                                maxLength={100}
                                className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                            </input>
                        </div>

                        <div
                            className='mb-4'>
                            <p
                                className='text-slate-800 text-lg font-semibold'>
                                <span className='superscript'>*</span>
                                Contact email
                                <VisibilityFlair text="sent with submitted applications"></VisibilityFlair>
                            </p>
                            <input
                                type="email"
                                name="contactEmail"
                                value={signupModel.contactEmail}
                                required
                                className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                            </input>
                        </div>
                    </form>

                    <hr
                        className='my-4'>
                    </hr>

                    <form
                        onChange={onFormChange}
                        className='relative mt-4'>
                        <div
                            className='mb-4 relative'>
                            <p
                                className='text-slate-800 text-lg font-semibold'>
                                <span className='superscript'>*</span>
                                Hats
                                <VisibilityFlair text="public"></VisibilityFlair>
                            </p>
                            <p
                                className='text-slate-500 text-sm text-justify'>
                                Tell us about the hats you wear! Are you a student, an academic, or both?
                                We'll use this information to find projects looking for a collaborator just like you.
                            </p>
                            <select
                                name="hatType"
                                value={selectedHatType}
                                className='block mt-2 rounded-md w-full h-12 p-2 text-base bg-white border border-slate-300 focus:outline-none focus:border-blue-500 focus:blue-500 text-slate-800 text-lg'>
                                <option value="Student">Student</option>
                                <option value="Academic">Academic / Researcher</option>
                            </select>
                        </div>
                    </form>

                    <HatForm
                        hatType={selectedHatType}
                        onHatAdded={addHat}>
                    </HatForm>

                    <div
                        className='mb-64 mt-8 relative'>
                        <p
                            className='text-slate-800 text-md font-semibold'>
                            Added hats
                        </p>

                        {
                            signupModel.hats.length === 0 &&
                            <p
                                className='text-slate-500 text-sm'>
                                No hats added
                            </p>
                        }

                        {
                            !validation.hats.valid &&
                            <p
                                className='text-red-700 text-sm italic'>
                                {validation.hats.errorMessage}
                            </p>
                        }

                        {
                            signupModel.hats.map(hat => (
                                <div
                                    key={Math.floor(Math.random() * 100000)}
                                    className="relative">
                                    <Hat hat={hat}></Hat>

                                    <button
                                        type="button"
                                        onClick={() => { removeHat(hat) }}
                                        className='absolute top-2 right-2'>
                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 stroke-slate-800 hover:stroke-red-700">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                                        </svg>
                                    </button>
                                </div>))
                        }
                    </div>

                    <button
                        type="submit"
                        form="parent-form"
                        className='absolute right-0 bottom-16 px-4 py-2 rounded-md bg-blue-500 hover:bg-blue-600 text-stone-50 font-semibold text-lg'>
                        Sign up
                    </button>
                </>
            </SingleColumnLayout>
        </>
    )
}

export default Signup