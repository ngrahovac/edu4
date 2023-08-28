import React, { useRef } from 'react'
import { useState } from 'react';
import AddedPosition from '../comps/publish/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import { publish } from '../services/ProjectsService';
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react'
import BasicInfoForm from '../comps/publish/BasicInfoForm';
import PositionForm from '../comps/publish/PositionForm';
import SectionTitleWrapper from '../layout/SectionTitleWrapper';
import InvalidFormFieldWarning from '../comps/publish/InvalidFormFieldWarning';
import _ from 'lodash';
import { BeatLoader } from 'react-spinners';
import SpinnerLayout from '../layout/SpinnerLayout';

const Publish = () => {
    const [project, setProject] = useState({ title: '', description: '', positions: [] });
    const [position, setPosition] = useState(undefined);
    const [loading, setLoading] = useState(false);

    const validBasicInfo = project.title && project.description;
    const validPosition = position;
    const validPositionCount = project.positions.length > 0;
    const duplicatePositions = _.uniq(project.positions).length != project.positions.length;

    const startShowingValidationErrors = useRef(false);

    const { getAccessTokenWithPopup, getAccessTokenSilently } = useAuth0();

    function handleRemovePosition(position) {
        setProject({
            ...project,
            positions: project.positions.filter(p => p != position)
        });
    }

    function handlePublishProject() {
        (async () => {
            if (validBasicInfo && validPositionCount) {
                setLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await publish(project, token);
                    console.log("done with api call");

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
                } catch (ex) {
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } finally {
                    setLoading(false);
                }
            }
        })();
    }

    const children = (
        <div className='flex flex-col gap-y-24'>
            {/* basic info */}
            <div className='flex flex-row gap-x-24'>
                <div className='w-full'>
                    <SectionTitleWrapper>
                        <SectionTitle title="Basic info"></SectionTitle>
                        <p className='h-8'></p>
                    </SectionTitleWrapper>

                    <BasicInfoForm
                        initialBasicInfo={{ title: project.title, description: project.description }}
                        onValidChange={basicInfo => {
                            setProject({ ...project, ...basicInfo });
                            if (!startShowingValidationErrors.current) {
                                startShowingValidationErrors.current = true;
                            }
                        }}
                        onInvalidChange={() => {
                            setProject({ ...project, title: '', description: '' });
                            if (!startShowingValidationErrors.current) {
                                startShowingValidationErrors.current = true;
                            }
                        }}
                        startShowingValidationErrors={startShowingValidationErrors.current}>
                    </BasicInfoForm>
                </div>
                <div className='w-full'>
                </div>
            </div>

            <div className='flex flex-row gap-x-24 h-full'>
                {/* add position */}
                <div className='relative pb-16 w-full'>
                    <SectionTitleWrapper>
                        <SectionTitle title="Positions"></SectionTitle>
                        <p className='h-8'>Describe the positions you're looking for collaborators for</p>
                    </SectionTitleWrapper>

                    <PositionForm
                        onValidChange={position => {
                            setPosition(position);
                            if (!startShowingValidationErrors.current) {
                                startShowingValidationErrors.current = true;
                            }
                        }}
                        onInvalidChange={() => {
                            setPosition(undefined);
                            if (!startShowingValidationErrors.current) {
                                startShowingValidationErrors.current = true;
                            }
                        }}
                        startShowingValidationErrors={startShowingValidationErrors.current}>
                    </PositionForm>

                    <div className='absolute bottom-0 right-0'>
                        <NeutralButton
                            disabled={!validPosition}
                            text="Add"
                            onClick={() => setProject({ ...project, positions: [...project.positions, position] })}>
                        </NeutralButton>
                    </div>
                </div>

                {/* added positions + publish */}
                <div className='w-full relative'>
                    <div className='mt-24'>
                        <SubsectionTitle title="Added positions"></SubsectionTitle>
                        <InvalidFormFieldWarning
                            visible={startShowingValidationErrors.current && !validPositionCount}
                            text="Add at least one position when publishing a project.">
                        </InvalidFormFieldWarning>
                        <InvalidFormFieldWarning
                            visible={startShowingValidationErrors.current && duplicatePositions}
                            text="A project cannot contain duplicate positions.">
                        </InvalidFormFieldWarning>

                        <div className='absolute w-full bottom-16 top-52 overflow-y-auto'>
                            {
                                project.positions.length == 0 &&
                                <p className='text-gray-500'>There are currently no added positions.</p>
                            }
                            {
                                project.positions.length > 0 &&
                                <div className='flex flex-col space-y-4'>
                                    {
                                        project.positions.map((p, index) => (
                                            <div key={index}>
                                                <AddedPosition
                                                    position={p}
                                                    onRemoved={handleRemovePosition}>
                                                </AddedPosition>
                                            </div>)
                                        )
                                    }
                                </div>
                            }
                        </div>
                    </div>

                    {/* publish button */}
                    <div className='absolute bottom-0 right-0'>
                        <PrimaryButton
                            text="Publish"
                            onClick={handlePublishProject}
                            disabled={!(validBasicInfo && validPositionCount && !duplicatePositions)}>
                        </PrimaryButton>
                    </div>
                </div>
            </div>
        </div>
    );

    if (loading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={loading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        );
    }

    return (
        <DoubleColumnLayout
            title="Publish a project"
            description="Have an idea or a project you're working on? Describe the collaborators you need and find your people!">
            {children}
        </DoubleColumnLayout>
    );
}

export default Publish