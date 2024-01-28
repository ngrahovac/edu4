import React, { useState, useEffect, useRef } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import Collaborators from '../comps/project/Collaborators';
import Author from '../comps/project/Author';
import Collaborator from '../comps/project/Collaborator';
import { getById, remove } from '../services/ProjectsService'
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
import PositionCard from '../comps/discover/PositionCard';
import PositionCardWithApplyOption from '../comps/project/PositionCardWithApplyOption';
import PositionCardWithRevokeOption from '../comps/project/PositionCardWithRevokeOption';

const Project = () => {
    const { projectId } = useParams();

    // data to be displayed
    const [project, setProject] = useState(undefined);
    const [pageLoading, setPageLoading] = useState(true);
    const { getAccessTokenSilently } = useAuth0();
    const [fetchUpdatedDataSwitch, setFetchUpdatedDataSwitch] = useState(true);

    // interactive state
    const [selectedPosition, setSelectedPosition] = useState(undefined);
    const applyingEnabled = selectedPosition !== undefined;
    const deleteConfirmationDialogRef = useRef(null);
    const submitApplicationConfirmationDialogRef = useRef(null);
    const revokeApplicationConfirmationDialogRef = useRef(null);


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

    function handleDeleteProjectRequested() {
        deleteConfirmationDialogRef.current.showModal();
    }

    function handleDeleteProjectConfirmed() {
        (async () => {
            setPageLoading(true);

            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await remove(project.id, token);
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
            project && project.recommended && (
                <>
                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Recommended positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.filter(p => p.recommended).map((p, index) => <div key={index}>
                                    {
                                        !p.applied &&
                                        <PositionCardWithApplyOption
                                            position={p}
                                            onApply={() => {
                                                setSelectedPosition(p);
                                                handleSubmitApplicationRequested();
                                            }}>
                                        </PositionCardWithApplyOption>
                                    }
                                    {
                                        p.applied &&
                                        <PositionCardWithRevokeOption
                                            position={p}
                                            onRevoke={() => {
                                                setSelectedPosition(p);
                                                handleRevokeApplicationRequested();
                                            }}>
                                        </PositionCardWithRevokeOption>
                                    }
                                </div>)
                            }
                        </div>
                    </div>

                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Other positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.filter(p => !p.recommended).map((p, index) => <div key={index}>
                                    {
                                        !p.applied &&
                                        <PositionCardWithApplyOption
                                            position={p}
                                            onApply={() => {
                                                setSelectedPosition(p);
                                                handleSubmitApplicationRequested();
                                            }}>
                                        </PositionCardWithApplyOption>
                                    }

                                    {
                                        p.applied &&
                                        <PositionCardWithRevokeOption
                                            position={p}
                                            onRevoke={() => {
                                                setSelectedPosition(p);
                                                handleRevokeApplicationRequested();
                                            }}>
                                        </PositionCardWithRevokeOption>
                                    }
                                </div>)
                            }
                        </div>
                    </div>
                </>
            )
        }

        {
            project && !project.recommended && (
                <>
                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.map((p, index) => <div key={index}>
                                    {
                                        !p.applied &&
                                        <PositionCardWithApplyOption
                                            position={p}
                                            onApply={() => {
                                                setSelectedPosition(p);
                                                handleSubmitApplicationRequested();
                                            }}>
                                        </PositionCardWithApplyOption>
                                    }
                                    {
                                        p.applied &&
                                        <PositionCardWithRevokeOption
                                            position={p}
                                            onRevoke={() => {
                                                setSelectedPosition(p);
                                                handleRevokeApplicationRequested();
                                            }}>
                                        </PositionCardWithRevokeOption>
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
            <dialog ref={deleteConfirmationDialogRef}>
                <ConfirmationDialog
                    question="Are you sure you want to delete this project?"
                    description="You cannot undo this action"
                    onConfirm={handleDeleteProjectConfirmed}
                    onCancel={() => deleteConfirmationDialogRef.current.close()}>
                </ConfirmationDialog>
            </dialog>

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
                                    project.collaborations.length &&
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
                                !project.collaborations.length &&
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