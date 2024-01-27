import React, { useState, useEffect, useRef } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import ProjectDescriptor from '../comps/discover/ProjectDescriptor';
import { SectionTitle } from '../layout/SectionTitle';
import Collaborators from '../comps/project/Collaborators';
import Author from '../comps/project/Author';
import Collaborator from '../comps/project/Collaborator';
import BorderlessButton from '../comps/buttons/BorderlessButton';
import { getById, remove } from '../services/ProjectsService'
import { useAuth0 } from '@auth0/auth0-react';
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { Link, useParams } from 'react-router-dom';
import PrimaryButton from '../comps/buttons/PrimaryButton'
import { submitApplication } from '../services/ApplicationsService';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';
import ConfirmationDialog from '../comps/shared/ConfirmationDialog';
import SubsectionTitle from '../layout/SubsectionTitle';
import PositionCard from '../comps/discover/PositionCard';

const Project = () => {
    const { projectId } = useParams();

    // data to be displayed
    const [project, setProject] = useState(undefined);
    const [pageLoading, setPageLoading] = useState(true);
    const { getAccessTokenSilently } = useAuth0();

    // interactive state
    const [selectedPosition, setSelectedPosition] = useState(undefined);
    const applyingEnabled = selectedPosition !== undefined;
    const deleteConfirmationDialogRef = useRef(null);

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
    }, [getAccessTokenSilently, projectId]);

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

    function handleSubmitApplication() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await submitApplication(project.id, selectedPosition.id, token);

                if (result.outcome === successResult) {
                    console.log("success");
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
                                    <PositionCard position={p}></PositionCard>
                                </div>)
                            }
                        </div>
                    </div>

                    <div className='flex flex-col gap-y-4'>
                        <SubsectionTitle title="Other positions"></SubsectionTitle>
                        <div className='flex flex-col space-y-2'>
                            {
                                project.positions.filter(p => !p.recommended).map((p, index) => <div key={index}>
                                    <PositionCard position={p}></PositionCard>
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
                                    <PositionCard position={p}></PositionCard>
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

            {
                project &&
                <SingleColumnLayout
                    title={project.title}>

                    <div className='flex flex-col gap-y-8'>

                        <div className='flex flex-col gap-y-4'>
                            <SubsectionTitle title="Description"></SubsectionTitle>
                            <p>{project.description}</p>
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
                                            name={c.name}
                                            position={c.position}
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

                        {/*
                            project.authored &&
                            <div className='absolute top-8 right-0 flex flex-row space-x-8'>
                                <Link to="edit">
                                    <BorderlessButton
                                        text="Edit"
                                        icon={
                                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                                <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                                            </svg>
                                        }
                                        onClick={() => { }}>
                                    </BorderlessButton>
                                </Link>

                                <BorderlessButton
                                    text="Delete"
                                    icon={
                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
                                        </svg>
                                    }
                                    onClick={handleDeleteProjectRequested}>
                                </BorderlessButton>
                            </div>
                                */}
                    </div>

                    {/*
                        !project.authored &&
                        <div className='absolute bottom-16 right-0'>
                            <PrimaryButton
                                text="Apply"
                                onClick={handleSubmitApplication}
                                disabled={!applyingEnabled}>
                            </PrimaryButton>
                        </div>
                            */}
                </SingleColumnLayout>
            }
        </>
    )
}

export default Project