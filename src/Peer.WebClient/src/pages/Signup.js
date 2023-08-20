import React from 'react'
import { useState } from 'react'
import { signUp } from '../services/SignupService'
import { useAuth0 } from '@auth0/auth0-react'
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import PersonalInfoForm from '../comps/signup/PersonalInfoForm'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle'
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import HatForm from '../comps/hat-forms/HatForm'
import NeutralButton from '../comps/buttons/NeutralButton'
import PrimaryButton from '../comps/buttons/PrimaryButton'
import _ from 'lodash'
import AddedHat from '../comps/signup/AddedHat'

const Signup = () => {
    const { getAccessTokenSilently, getAccessTokenWithPopup, user, isAuthenticated } = useAuth0();

    const [signupModel, setSignupModel] = useState({
        contactEmail: "",
        fullName: "",
        hats: []
    });
    const [hat, setHat] = useState(undefined);

    const [validPersonalInfo, setValidPersonalInfo] = useState(true);
    const [validHat, setValidHat] = useState(false);

    function onSignupRequested() {
        (async () => {
            try {
                {/* validation of other parts of the model is handled on HTML level */ }
                if (validPersonalInfo && signupModel.hats.length > 0) {
                    let token = await getAccessTokenWithPopup({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await signUp(signupModel, token);

                    if (result.outcome === successResult) {
                        // document.getElementById('user-action-success-toast').show();
                        // setTimeout(() => window.location.href = "/homepage", 1000);
                    } else if (result.outcome === failureResult) {
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    } else if (result.outcome === errorResult) {
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    }
                }
            } catch (ex) {
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    const left = (
        <>
            <div className='mb-8'>
                <SectionTitle title="Personal info"></SectionTitle>
                <p className='h-8'></p>
            </div>

            <PersonalInfoForm
                user={user}
                onValidChange={personalInfo => {
                    setSignupModel({ ...signupModel, ...personalInfo });
                    setValidPersonalInfo(true);
                }}
                onInvalidChange={() => setValidPersonalInfo(false)}>
            </PersonalInfoForm>
        </>
    );

    const right = (
        <>
            <div className='relative pb-32'>
                <div className='relative pb-16'>
                    <div className="mb-8">
                        <SectionTitle title="Hats"></SectionTitle>
                        <p className='h-8'>Are you a student, researcher, or both? We will use this information to find projects looking for someone like you.</p>
                    </div>

                    <HatForm
                        onValidChange={hat => {
                            setHat(hat);
                            setValidHat(true);
                        }}
                        onInvalidChange={() => setValidHat(false)}>
                    </HatForm>

                    <div className='absolute bottom-0 right-0'>
                        <NeutralButton
                            disabled={!validHat}
                            text="Add"
                            onClick={() => {
                                if (validHat)
                                    setSignupModel({ ...signupModel, hats: [...signupModel.hats, hat] })
                            }}>
                        </NeutralButton>
                    </div>
                </div>

                <div className='mb-2'>
                    <SubsectionTitle title="Added hats"></SubsectionTitle>
                    <p className='text-red-500 font-semibold h-8'>{`${signupModel.hats.length > 0 ? "" : "Specify at least one hat"}`}</p>
                </div>
                {
                    signupModel.hats.length == 0 &&
                    <p className='text-gray-500'>There are currently no added hats.</p>
                }
                {
                    signupModel.hats.length > 0 &&
                    <div className="mt-4"> {
                        signupModel.hats.map(h => (
                            <div key={Math.random() * 1000}>
                                <div className='mb-2'>
                                    <AddedHat
                                        hat={h}
                                        onRemoved={(removedHat) => {
                                            setSignupModel({
                                                ...signupModel,
                                                hats: signupModel.hats.filter(h => !_.isEqual(h, removedHat))
                                            }
                                            )
                                        }}>
                                    </AddedHat>
                                </div>
                            </div>)
                        )
                    }
                    </div>
                }

                <div className='absolute bottom-2 right-0'>
                    <PrimaryButton
                        text="Sign up"
                        onClick={onSignupRequested}
                        disabled={!(validPersonalInfo && signupModel.hats.length > 0)}>
                    </PrimaryButton>
                </div>
            </div>
        </>
    );

    return (
        isAuthenticated &&
        <DoubleColumnLayout
            title="Tell us about yourself"
            left={left}
            right={right}>
        </DoubleColumnLayout>
    )
}

export default Signup