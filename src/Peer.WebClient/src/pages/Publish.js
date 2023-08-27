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

const Publish = () => {
    const [project, setProject] = useState({ positions: [] });
    const [position, setPosition] = useState({});

    const validBasicInfo = project.title && position;
    const validPosition = position;
    const validPositionCount = project.positions.length > 0;

    const startShowingValidationErrors = useRef(false);

    const { getAccessTokenWithPopup } = useAuth0();

    function handleRemovePosition(position) {
        setProject({
            ...project,
            positions: project.positions.filter(p => p != position)
        });
    }

    function handlePublishProject() {
        (async () => {
            if (validBasicInfo && validPositionCount) {
                try {
                    let token = await getAccessTokenWithPopup({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await publish(project, token);

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
                }
            }
        })();
    }

    const left = (
        <>
            <SectionTitleWrapper>
                <SectionTitle title="Basic info"></SectionTitle>
                <p className='h-8'></p>
            </SectionTitleWrapper>

            <BasicInfoForm
                onValidChange={basicInfo => {
                    setProject({ ...project, ...basicInfo });
                    if (!startShowingValidationErrors.current) {
                        startShowingValidationErrors.current = true;
                    }
                }}
                onInvalidChange={() => {
                    setProject({ ...project, title: undefined, description: undefined });
                    if (!startShowingValidationErrors.current) {
                        startShowingValidationErrors.current = true;
                    }
                }}
                startShowingValidationErrors={startShowingValidationErrors.current}>
            </BasicInfoForm>
        </>
    );

    const right = (
        <div className='relative pb-16 flex flex-col space-y-8'>
            {/* add position */}
            <div className='relative pb-16'>
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

                <div className='absolute bottom-2 right-0'>
                    <NeutralButton
                        disabled={!validPosition}
                        text="Add"
                        onClick={() => setProject({ ...project, positions: [...project.positions, position] })}>
                    </NeutralButton>
                </div>
            </div>

            {/* added positions */}
            <div>
                <SectionTitleWrapper>
                    <SubsectionTitle title="Added positions"></SubsectionTitle>
                    <InvalidFormFieldWarning
                        visible={startShowingValidationErrors.current && !validPositionCount}
                        text="Add at least one position when publishing a project.">
                    </InvalidFormFieldWarning>
                </SectionTitleWrapper>
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

            {/* publish button */}
            <div className='absolute bottom-2 right-0'>
                <PrimaryButton
                    text="Publish"
                    onClick={handlePublishProject}
                    disabled={!(validBasicInfo && validPositionCount)}>
                </PrimaryButton>
            </div>
        </div >
    );

    return (
        <DoubleColumnLayout
            title="Publish a project"
            description="Have an idea or a project you're working on? Describe the collaborators you need and find your people!"
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default Publish