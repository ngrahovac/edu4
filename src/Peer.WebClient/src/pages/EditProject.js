import React, { useEffect } from 'react'
import { useState } from 'react';
import AddedPosition from '../comps/publish/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import { addPositions, closePosition, removePosition, reopenPosition, updateDetails } from '../services/ProjectsService';
import { getById } from '../services/ProjectsService'

import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react'
import { useParams } from 'react-router-dom';
import BasicInfoForm from '../comps/publish/BasicInfoForm';
import ProjectPositions from '../comps/project/ProjectPositions';
import PositionForm from '../comps/publish/PositionForm';
import DangerButton from '../comps/buttons/DangerButton';
import { BeatLoader } from 'react-spinners';
import SpinnerLayout from '../layout/SpinnerLayout';
import { useRef } from 'react';
import ConfirmationDialog from '../comps/shared/ConfirmationDialog';

const EditProject = () => {

    const { projectId } = useParams();

    const removePositionConfirmationDialogRef = useRef(null);

    const [originalProject, setOriginalProject] = useState(undefined);
    const [project, setProject] = useState(undefined);
    const [position, setPosition] = useState(undefined);

    const validPosition = position;
    const validBasicInfo = project && project.title && project.description;

    const [newPositions, setNewPositions] = useState([]);
    const [selectedPosition, setSelectedPosition] = useState(undefined);

    const [pageLoading, setPageLoading] = useState(true);

    const [startShowingValidationErrors, setStartShowingValidationErrors] = useState(false);

    const { getAccessTokenSilently } = useAuth0();

    function fetchProject() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getById(projectId, token);
                setPageLoading(false);

                if (result.outcome === successResult) {
                    var project = result.payload;

                    // sort positions by recommended first
                    const recommendedPositionSorter = (a, b) => {
                        if (a.recommended && !b.recommended) return -1;
                        if (!a.recommended && b.recommended) return +1;
                        return 0;
                    };

                    project.positions.sort(recommendedPositionSorter);

                    setOriginalProject(project);
                    setProject(project);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error");
                }
            } catch (ex) {
                console.log("exception", ex);
            }
        })();
    }

    useEffect(() => {
        fetchProject();
    }, [projectId]);

    function handleNewPositionRemoved(positionToRemove) {
        let filteredPositions = newPositions.filter(p => p !== positionToRemove);
        setNewPositions(filteredPositions);
    }

    function handleAddPositions() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await addPositions(project.id, newPositions, token);
                setPageLoading(false);

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
                setPageLoading(false);
            }
        })();
    }

    function handleUpdateDetails() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await updateDetails(project.id, project.title, project.description, token);
                setPageLoading(false);

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
                setPageLoading(false);
            }
        })();
    }

    function handlePositionClosed() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await closePosition(project.id, selectedPosition.id, token)
                setPageLoading(false);

                if (result.outcome === successResult) {
                    setSelectedPosition({ ...selectedPosition, open: false });
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error");
                }
            } catch (ex) {
                console.log("exception", ex);
            } finally {
                setPageLoading(false);
            }
        })();
    }

    function handlePositionReopened() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await reopenPosition(project.id, selectedPosition.id, token)
                setPageLoading(false);

                if (result.outcome === successResult) {
                    setSelectedPosition({ ...selectedPosition, open: true });
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error");
                }
            } catch (ex) {
                console.log("exception", ex);
            } finally {
                setPageLoading(false);
            }
        })();
    }

    function handleRemoveExistingPositionConfirmed() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await removePosition(project.id, selectedPosition.id, token)
                setPageLoading(false);

                if (result.outcome === successResult) {
                    setProject({ ...project, positions: project.positions.filter(p => p.id !== selectedPosition.id) })
                    setSelectedPosition(undefined);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error");
                }
            } catch (ex) {
                console.log("exception", ex);
            } finally {
                setPageLoading(false);
            }
        })();
    }

    function handleRemoveExistingPositionRequested() {
        removePositionConfirmationDialogRef.current.showModal();
    }

    const children = (
        project &&
        <>
            <dialog ref={removePositionConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Are you sure you want to remove this position?"
                    description="It will no longer be visible on the project page and other collaborators will not be able to apply. You cannot undo this action."
                    onConfirm={handleRemoveExistingPositionConfirmed}
                    onCancel={() => removePositionConfirmationDialogRef.current.close()}>
                </ConfirmationDialog>
            </dialog>

            <div className='relative pb-16'>
                <div className='mb-8'>
                    <SectionTitle title="Basic info"></SectionTitle>
                    <p className='h-8'></p>
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


                <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                    <NeutralButton
                        text="Cancel"
                        onClick={() => { setProject({ ...project, title: originalProject.title, description: originalProject.description }) }}>
                    </NeutralButton>

                    <PrimaryButton
                        text="Update details"
                        onClick={handleUpdateDetails}
                        disabled={!(validBasicInfo)}>
                    </PrimaryButton>
                </div>
            </div>

            <div className='relative pb-32'>
                <div className="mb-12">
                    <SectionTitle title="Positions"></SectionTitle>
                    <p>Describe the profiles of people you're looking to find and collaborate with</p>
                </div>

                {/* manage existing positions */}
                <div className='relative mb-12 pb-16'>
                    <div className='mb-8'>
                        <SubsectionTitle title="Manage existing positions"></SubsectionTitle>
                    </div>
                    {
                        project.positions.length === 0 &&
                        <p className='text-gray-500'>Currently there are no added positions.</p>
                    }
                    {
                        project.positions.length > 0 &&
                        <div className="mt-4">
                            <ProjectPositions
                                positions={project.positions}
                                selectionEnabled={true}
                                onSelectedPositionChanged={(position) => { setSelectedPosition(position); }}>
                            </ProjectPositions>
                        </div>
                    }

                    {
                        project.positions.length > 0 &&
                        <div className='absolute bottom-0 right-0 flex flex-row space-x-2'>
                            <DangerButton
                                text="Close"
                                disabled={!selectedPosition || !selectedPosition.open}
                                onClick={handlePositionClosed}>
                            </DangerButton>

                            <NeutralButton
                                text="Reopen"
                                disabled={!selectedPosition || selectedPosition.open}
                                onClick={handlePositionReopened}>
                            </NeutralButton>

                            <DangerButton
                                text="Remove"
                                disabled={!selectedPosition}
                                onClick={handleRemoveExistingPositionRequested}>
                            </DangerButton>
                        </div>
                    }
                </div>

                {/* add a new position */}
                <div className='relative mb-8 pb-16'>
                    <div className='mb-4'>
                        <SubsectionTitle title="Add a new position"></SubsectionTitle>
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
                            onClick={() => {
                                if (validPosition)
                                    setNewPositions([...newPositions, position])
                            }}>
                        </NeutralButton>
                    </div>
                </div>

                {/* new positions */}
                <div>
                    <div className='mb-4 mt-12'>
                        <SubsectionTitle title="New positions"></SubsectionTitle>
                    </div>
                    {
                        newPositions.length === 0 &&
                        <p className='text-gray-500'>Currently there are no added positions.</p>
                    }
                    {
                        newPositions.map(p => (
                            <div key={Math.random() * 1000}>
                                <div className='mb-2'>
                                    {/*  <Position position={p}></Position> */}
                                    <AddedPosition
                                        position={p}
                                        onRemoved={() => handleNewPositionRemoved(p)}>
                                    </AddedPosition>
                                </div>
                            </div>)
                        )
                    }

                    <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                        <NeutralButton
                            text="Cancel"
                            onClick={() => { setNewPositions([]) }}>
                        </NeutralButton>

                        <PrimaryButton
                            text="Update positions"
                            onClick={handleAddPositions}
                            disabled={newPositions.length < 1}>
                        </PrimaryButton>
                    </div>
                </div>
            </div>
        </>

    );

    if (pageLoading) {
        return <SpinnerLayout>
            <BeatLoader></BeatLoader>
        </SpinnerLayout>
    }

    return (
        originalProject !== undefined &&
        <DoubleColumnLayout
            title="Edit project"
            description="">
            {children}
        </DoubleColumnLayout>
    );
}

export default EditProject