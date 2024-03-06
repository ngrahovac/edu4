import React, { useState, useEffect, useRef } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import Author from '../comps/project/Author';
import Collaborator from '../comps/project/Collaborator';
import { closePosition, getById, remove, removePosition, reopenPosition } from '../services/ProjectsService'
import { useAuth0 } from '@auth0/auth0-react';
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useParams } from 'react-router-dom';
import { revokeApplication, submitApplication } from '../services/ApplicationsService';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';
import ConfirmationDialog from '../comps/shared/ConfirmationDialog';
import SubsectionTitle from '../layout/SubsectionTitle';
import PositionCardWithAuthorOptions from '../comps/project/PositionCardWithAuthorOptions';
import PositionCardWithCollaboratorOptions from '../comps/project/PositionCardWithCollaboratorOptions';
import DangerTertiaryButton from '../comps/buttons/DangerTertiaryButton';
import { me } from '../services/UsersService'
import Collaborators from '../comps/temp/Collaborators';

const Project = () => {
    const { projectId } = useParams();

    // data to be displayed
    const [project, setProject] = useState(undefined);
    const [pageLoading, setPageLoading] = useState(true);
    const { getAccessTokenSilently } = useAuth0();
    const [fetchUpdatedDataSwitch, setFetchUpdatedDataSwitch] = useState(true);
    const [ownHats, setOwnHats] = useState(undefined);

    // interactive state
    const [selectedPosition, setSelectedPosition] = useState(undefined);
    const submitApplicationConfirmationDialogRef = useRef(null);
    const revokeApplicationConfirmationDialogRef = useRef(null);
    const removePositionConfirmationDialogRef = useRef(null);
    const deleteProjectConfirmationDialogRef = useRef(null);

    useEffect(() => {
        const fetchProject = () => {
            (async () => {
                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await getById(projectId, token);

                    if (result.outcome === successResult) {
                        var project = result.payload;

                        // sort positions by recommended first
                        const recommendedPositionSorter = (a, b) => {
                            if (a.recommended && !b.recommended) return -1;
                            if (!a.recommended && b.recommended) return +1;
                            return 0;
                        };

                        project.positions.sort(recommendedPositionSorter);

                        setProject(project);
                    }
                } catch (ex) {
                    console.log(ex);
                }
            })();
        }

        setPageLoading(true);
        fetchProject();
        setPageLoading(false);
    }, [getAccessTokenSilently, projectId, fetchUpdatedDataSwitch]);

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setPageLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setPageLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setPageLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [])

    function handleSubmitApplicationRequested() {
        submitApplicationConfirmationDialogRef.current.showModal();
    }

    function handleSubmitApplication() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await submitApplication(project.id, selectedPosition.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    submitApplicationConfirmationDialogRef.current.close();
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

    function handleRevokeApplicationRequested() {
        revokeApplicationConfirmationDialogRef.current.showModal();
    }

    function handleRevokeApplication() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let applicationId = project.applications.find(a => a.positionId == selectedPosition.id).id;
                let result = await revokeApplication(applicationId, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    revokeApplicationConfirmationDialogRef.current.close();
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

    function handleClosePosition() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await closePosition(project.id, selectedPosition.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    removePositionConfirmationDialogRef.current.close();
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

    function handleReopenPosition() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await reopenPosition(project.id, selectedPosition.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    removePositionConfirmationDialogRef.current.close();
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

    function handleRemovePositionRequested() {
        removePositionConfirmationDialogRef.current.showModal();
    }

    function handleRemovePosition() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await removePosition(project.id, selectedPosition.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    removePositionConfirmationDialogRef.current.close();
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

    function handleDeleteProjectRequested() {
        deleteProjectConfirmationDialogRef.current.showModal();
    }

    function handleDeleteProject() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await remove(project.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    setFetchUpdatedDataSwitch(!fetchUpdatedDataSwitch);
                    setSelectedPosition(undefined);
                    removePositionConfirmationDialogRef.current.close();
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

    if (pageLoading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={pageLoading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        );
    }

    const positions = <>
        {
            project && project.authored && (
                <div className='flex flex-col gap-y-4'>
                    <SubsectionTitle title="Positions"></SubsectionTitle>
                    <div className='flex flex-col space-y-2'>
                        {
                            project.positions.map((p, index) => <div key={index}>
                                <PositionCardWithAuthorOptions
                                    position={p}
                                    onRemove={() => {
                                        setSelectedPosition(p);
                                        handleRemovePositionRequested();
                                    }}
                                    onClose={() => {
                                        setSelectedPosition(p);
                                        handleClosePosition();
                                    }}
                                    onReopen={() => {
                                        setSelectedPosition(p);
                                        handleReopenPosition();
                                    }}>
                                </PositionCardWithAuthorOptions>
                            </div>)
                        }
                    </div>
                </div>
            )
        }
        {
            project && !project.authored && project.recommended && (
                <>
                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Recommended positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.filter(p => p.recommended).map((p, index) => <div key={index}>
                                    <PositionCardWithCollaboratorOptions
                                        position={p}
                                        applied={project.applications?.find(a => a.positionId == p.id) != undefined}
                                        onApply={() => {
                                            setSelectedPosition(p);
                                            handleSubmitApplicationRequested();
                                        }}
                                        onRevoke={() => {
                                            setSelectedPosition(p);
                                            handleRevokeApplicationRequested();
                                        }}
                                        ownHats={ownHats}>
                                    </PositionCardWithCollaboratorOptions>
                                </div>)
                            }
                        </div>
                    </div>

                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Other positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.filter(p => !p.recommended).map((p, index) => <div key={index}>
                                    <PositionCardWithCollaboratorOptions
                                        position={p}
                                        applied={project.applications?.find(a => a.positionId == p.id) != undefined}
                                        onApply={() => {
                                            setSelectedPosition(p);
                                            handleSubmitApplicationRequested();
                                        }}
                                        onRevoke={() => {
                                            setSelectedPosition(p);
                                            handleRevokeApplicationRequested();
                                        }}
                                        ownHats={ownHats}>
                                    </PositionCardWithCollaboratorOptions>
                                </div>)
                            }
                        </div>
                    </div>
                </>
            )
        }
        {
            project && !project.authored && !project.recommended && (
                <>
                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.map((p, index) => <div key={index}>
                                    {
                                        <PositionCardWithCollaboratorOptions
                                            position={p}
                                            onApply={() => {
                                                setSelectedPosition(p);
                                                handleSubmitApplicationRequested();
                                            }}
                                            onRevoke={() => {
                                                setSelectedPosition(p);
                                                handleRevokeApplicationRequested();
                                            }}>
                                        </PositionCardWithCollaboratorOptions>
                                    }
                                </div>)
                            }
                        </div>
                    </div>
                </>
            )
        }
    </>

    return (
        <>
            <dialog ref={submitApplicationConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Submit application"
                    description="By applying for this position, you'll be eligible for consideration as a project collaborator"
                    onCancel={() => submitApplicationConfirmationDialogRef.current.close()}
                    onConfirm={handleSubmitApplication}>
                </ConfirmationDialog>
            </dialog>

            <dialog ref={revokeApplicationConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Revoke application"
                    description="You cannot undo this action"
                    onCancel={() => revokeApplicationConfirmationDialogRef.current.close()}
                    onConfirm={handleRevokeApplication}>
                </ConfirmationDialog>
            </dialog>

            <dialog ref={removePositionConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Remove position"
                    description="You cannot undo this action"
                    onCancel={() => removePositionConfirmationDialogRef.current.close()}
                    onConfirm={handleRemovePosition}>
                </ConfirmationDialog>
            </dialog>

            <dialog ref={deleteProjectConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Delete project"
                    description="You cannot undo this action"
                    onCancel={() => deleteProjectConfirmationDialogRef.current.close()}
                    onConfirm={handleDeleteProject}>
                </ConfirmationDialog>
            </dialog>


            {
                project &&
                <SingleColumnLayout
                    title={project.title}>

                    <div className='flex flex-col gap-y-8'>
                        <div className='flex flex-col gap-y-4'>
                            <SubsectionTitle title="Description"></SubsectionTitle>
                            <p className='text-gray-600'>{project.description}</p>
                        </div>

                        <div className='flex flex-col gap-y-4'>
                            <SubsectionTitle title="Duration"></SubsectionTitle>
                            <div className='flex gap-x-2 text-gray-600'>
                                <span>{project.duration.startDate}</span>
                                <span>-</span>
                                <span>{project.duration.endDate}</span>
                            </div>
                        </div>

                        {positions}

                        <div className='flex flex-col gap-y-4'>
                            <SubsectionTitle title="Collaborators"></SubsectionTitle>
                            {
                                !project.collaborations.length > 0 &&
                                <p>There are no other collaborators on this project.&nbsp;
                                    {
                                        !project.authored &&
                                        <span className='italic'>Apply to a position for a chance to be the first one.
                                        </span>
                                    }
                                </p>
                            }
                        </div>

                        {
                            project.authored &&
                            <div className='flex flex-row-reverse py-16'>
                                <DangerTertiaryButton
                                    text="Delete project"
                                    icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
                                    </svg>
                                    }
                                    onClick={handleDeleteProjectRequested}>
                                </DangerTertiaryButton>
                            </div>
                        }
                    </div>
                </SingleColumnLayout>
            }
        </>
    )
}

export default Project