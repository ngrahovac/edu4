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
import ConfirmationDialog from '../comps/shared/ConfirmationDialog';
import SingleColumnLayout from '../layout/SingleColumnLayout';

const Publish = () => {
    const [project, setProject] = useState({ title: '', description: '', positions: [] });
    const [position, setPosition] = useState(undefined);
    const [loading, setLoading] = useState(false);

    const validBasicInfo = project.title && project.description;
    const validPosition = position;
    const validPositionCount = project.positions.length > 0;
    const duplicatePositions = _.uniq(project.positions).length !== project.positions.length;

    const [startShowingValidationErrors, setStartShowingValidationErrors] = useState(false);

    const publishConfirmationDialogRef = useRef(null);

    const { getAccessTokenSilently } = useAuth0();

    function handleRemovePosition(position) {
        setProject({
            ...project,
            positions: project.positions.filter(p => p !== position)
        });
    }

    function handlePublishProjectRequested() {
        publishConfirmationDialogRef.current.showModal();
    }

    function handlePublishProjectConfirmed() {
        (async () => {
            if (validBasicInfo && validPositionCount) {
                setLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await publish(project, token);
                    setLoading(false);

                    if (result.outcome === successResult) {
                        console.log("success");
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setLoading(false);
                }
            }
        })();
    }

    const left = (
        <>
            <dialog ref={publishConfirmationDialogRef}>
                <ConfirmationDialog
                    ref={publishConfirmationDialogRef}
                    question="Are you sure you want to publish this project?"
                    description="After publishing, it will become visible to other contributors."
                    onConfirm={handlePublishProjectConfirmed}
                    onCancel={() => publishConfirmationDialogRef.current.close()}>
                </ConfirmationDialog>
            </dialog>

            <div className='flex flex-col gap-y-8 w-full'>
                {/* basic info */}
                <div className='w-full flex flex-col gap-y-6'>
                    <div className='flex flex-col gap-y-2'>
                        <p className='text-lg text-gray-700'>Basic info</p>
                    </div>

                    <BasicInfoForm
                        initialBasicInfo={{ title: project.title, description: project.description }}
                        onValidChange={basicInfo => {
                            setProject({ ...project, ...basicInfo });
                            if (!startShowingValidationErrors) {
                                setStartShowingValidationErrors(true);
                            }
                        }}
                        onInvalidChange={() => {
                            setProject({ ...project, title: '', description: '' });
                            if (!startShowingValidationErrors) {
                                setStartShowingValidationErrors(true);
                            }
                        }}
                        startShowingValidationErrors={startShowingValidationErrors}>
                    </BasicInfoForm>
                </div>
            </div>
        </>
    );

    const right = (
        <div className='flex flex-col w-full'>
            {/* add position */}
            <div className='relative pb-16 w-full'>
                <div className='flex flex-col gap-y-2'>
                <p className='text-lg text-gray-700'>Positions</p>
                    <p className='h-8'></p>
                </div>

                <PositionForm
                    onValidChange={position => {
                        setPosition(position);
                        if (!startShowingValidationErrors) {
                            setStartShowingValidationErrors(true);
                        }
                    }}
                    onInvalidChange={() => {
                        setPosition(undefined);
                        if (!startShowingValidationErrors) {
                            setStartShowingValidationErrors(true);
                        }
                    }}
                    startShowingValidationErrors={startShowingValidationErrors}>
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
                    <p className='text-lg text-gray-700'>Added positions</p>
                    <InvalidFormFieldWarning
                        visible={startShowingValidationErrors && !validPositionCount}
                        text="Add at least one position when publishing a project.">
                    </InvalidFormFieldWarning>
                    <InvalidFormFieldWarning
                        visible={startShowingValidationErrors && duplicatePositions}
                        text="A project cannot contain duplicate positions.">
                    </InvalidFormFieldWarning>

                    <div className='absolute w-full bottom-16 top-52 overflow-y-auto'>
                        {
                            project.positions.length === 0 &&
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
                        onClick={handlePublishProjectRequested}
                        disabled={!(validBasicInfo && validPositionCount && !duplicatePositions)}>
                    </PrimaryButton>
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
            description="Have an idea or a project you're working on? Describe the collaborators you need and find your people!"
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default Publish