import React, { useState, useEffect, useRef } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import Collaborators from '../comps/project/Collaborators';
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

const Project = () => {
    const { projectId } = useParams();

    // data to be displayed
    const [project, setProject] = useState(undefined);
    const [pageLoading, setPageLoading] = useState(true);
    const { getAccessTokenSilently } = useAuth0();
    const [fetchUpdatedDataSwitch, setFetchUpdatedDataSwitch] = useState(true);

    // interactive state
    const [selectedPosition, setSelectedPosition] = useState(undefined);
    const submitApplicationConfirmationDialogRef = useRef(null);
    const revokeApplicationConfirmationDialogRef = useRef(null);
    const removePositionConfirmationDialogRef = useRef(null);

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

                let result = await revokeApplication(selectedPosition.applicationId, token);

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
                                        onApply={() => {
                                            setSelectedPosition(p);
                                            handleSubmitApplicationRequested();
                                        }}
                                        onRevoke={() => {
                                            setSelectedPosition(p);
                                            handleRevokeApplicationRequested();
                                        }}>
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
                                        onApply={() => {
                                            setSelectedPosition(p);
                                            handleSubmitApplicationRequested();
                                        }}
                                        onRevoke={() => {
                                            setSelectedPosition(p);
                                            handleRevokeApplicationRequested();
                                        }}>
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
                            <Collaborators>
                                <Author
                                    name={project.author.fullName}>
                                </Author>

                                {
                                    project.collaborations.length > 0 &&
                                    project.collaborations.map(c => <div key={c.id}>
                                        <Collaborator
                                            name={c.collaborator.fullName}
                                            position={project.positions.find(p => p.id == c.positionId).name}
                                            onVisited={() => { }}>
                                        </Collaborator>
                                    </div>)
                                }
                            </Collaborators>

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
                    </div>
                </SingleColumnLayout>
            }
        </>
    )
}

export default Project