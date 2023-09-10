import React, { useState, useEffect } from 'react'
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
import { getContributor } from '../services/UsersService';
import { getCollaborations } from '../services/CollaboratorsService';
import PrimaryButton from '../comps/buttons/PrimaryButton'
import ProjectPositions from '../comps/project/ProjectPositions';
import { submitApplication } from '../services/ApplicationsService';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';

const Project = () => {
    const { projectId } = useParams();

    const avatar = "https://www.gravatar.com/avatar/93e9084aa289b7f1f5e4ab6716a56c3b?s=80";
    const [project, setProject] = useState(undefined);
    const [author, setAuthor] = useState(undefined);
    const [collaborations, setCollaborations] = useState(undefined);
    const [selectedPosition, setSelectedPosition] = useState(undefined);
    const applyingEnabled = !selectedPosition;

    const [pageLoading, setPageLoading] = useState(true);

    const { getAccessTokenWithPopup } = useAuth0();

    function fetchAuthor() {
        (async () => {
            try {
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getContributor(token, project.authorUrl);

                if (result.outcome === successResult) {
                    var author = result.payload;
                    setAuthor(author);
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

    function fetchCollaborations() {
        (async () => {
            try {
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getCollaborations(project.collaborationsUrl, token);

                if (result.outcome === successResult) {
                    var collaborations = result.payload;
                    setCollaborations(collaborations);
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

    function fetchProject() {
        (async () => {
            setPageLoading(true);
            
            try {
                let token = await getAccessTokenWithPopup({
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

                    setProject(project);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error", result);
                }
            } catch (ex) {
                console.log("exception", ex);
            } finally {
                setPageLoading(false);
            }
        })();
    }

    useEffect(() => {
        fetchProject();
    }, [projectId]);

    useEffect(() => {
        if (project !== undefined) {
            fetchCollaborations();
        }
    }, [project]);

    useEffect(() => {
        if (project !== undefined) {
            fetchAuthor();
        }
    }, [project]);


    function onDeleteProject() {
        (async () => {
            try {
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await remove(project.id, token);

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

    function onSubmitApplication() {
        (async () => {
            try {
                let token = await getAccessTokenWithPopup({
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

    return (
        <>
            {
                project !== undefined &&
                author !== undefined &&

                <SingleColumnLayout
                    title={project.title}>

                    <div className='flex flex-col space-y-16'>
                        {/* project descriptors */}
                        <div className='flex flex-row flex-wrap space-x-6 absolute top-40'>
                            <Link to={project.authorUrl}>
                                <ProjectDescriptor
                                    value={author.fullName}
                                    link={true}
                                    icon=
                                    {
                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                                        </svg>
                                    }>
                                </ProjectDescriptor>
                            </Link>

                            <ProjectDescriptor
                                value={project.datePosted}
                                icon=
                                {<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5m-9-6h.008v.008H12v-.008zM12 15h.008v.008H12V15zm0 2.25h.008v.008H12v-.008zM9.75 15h.008v.008H9.75V15zm0 2.25h.008v.008H9.75v-.008zM7.5 15h.008v.008H7.5V15zm0 2.25h.008v.008H7.5v-.008zm6.75-4.5h.008v.008h-.008v-.008zm0 2.25h.008v.008h-.008V15zm0 2.25h.008v.008h-.008v-.008zm2.25-4.5h.008v.008H16.5v-.008zm0 2.25h.008v.008H16.5V15z" />
                                </svg>}>
                            </ProjectDescriptor>
                        </div>

                        {/* description */}
                        <div className='space-y-1'>
                            <SectionTitle title="Description"></SectionTitle>
                            <p>{project.description}</p>
                        </div>

                        {/* collaborators */}
                        <div className='space-y-4'>
                            <SectionTitle title="Collaborators"></SectionTitle>
                            <Collaborators>
                                {
                                    author !== undefined &&
                                    <Author
                                        name={author.fullName}>
                                    </Author>
                                }

                                {
                                    (collaborations !== undefined && collaborations.length >= 0) &&
                                    collaborations.map(c => <div key={c.id}>
                                        <Collaborator
                                            avatar={avatar}
                                            name={c.name}
                                            position={c.position}
                                            onVisited={() => { }}>
                                        </Collaborator>
                                    </div>)
                                }
                            </Collaborators>

                            {
                                (collaborations === undefined || collaborations.length === 0) &&
                                <>
                                    <p>There are no other collaborators on this project.&nbsp;
                                        {
                                            !project.authored &&
                                            <span className='italic'>Apply to a position for a chance to be the first one.
                                            </span>
                                        }
                                    </p>
                                </>
                            }
                        </div>

                        {/* positions */}
                        <div className='mt-4'>
                            <div>
                                <SectionTitle title="Open positions"></SectionTitle>
                                <ProjectPositions
                                    selectionEnabled={!project.authored}
                                    positions={project.positions}
                                    onSelectedPositionChanged={(position) => { setSelectedPosition(position); }}>
                                </ProjectPositions>
                            </div>
                        </div>

                        {
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
                                    onClick={onDeleteProject}>
                                </BorderlessButton>
                            </div>
                        }
                    </div>

                    {
                        !project.authored &&
                        <div className='absolute bottom-16 right-0'>
                            <PrimaryButton
                                text="Apply"
                                onClick={onSubmitApplication}
                                disabled={!applyingEnabled}>
                            </PrimaryButton>
                        </div>
                    }
                </SingleColumnLayout>
            }
        </>
    )
}

export default Project