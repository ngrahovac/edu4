import React, { useEffect, useRef, useState } from 'react'
import { useAuth0 } from '@auth0/auth0-react';
import PrimaryButton from '../buttons/PrimaryButton';
import { acceptApplication, rejectApplication } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
import { getAuthored } from '../../services/ProjectsService';
import DangerButton from '../buttons/DangerButton';
import ProjectFilter from './ProjectFilter';
import ApplicationsSorter from './SentApplicationsSorter';
import { BeatLoader } from 'react-spinners';
import ConfirmationDialog from '../shared/ConfirmationDialog';
import ApplicationsTable from '../table2/ApplicationsTable';
import { Link } from 'react-router-dom';
import SubmittedApplicationStatus from './SubmittedApplicationStatus';
import { ClipLoader } from 'react-spinners';
import TertiaryButton from '../buttons/TertiaryButton';
import BorderlessButton from '../buttons/BorderlessButton';


const ReceivedApplications = (props) => {
    const {
        applications: displayedApplicationsPage,
        onProjectIdFilterChanged,
        onSortChanged,
        onPageChanged = () => { }
    } = props;

    const [toggleReload, setToggleReload] = useState(false);

    const [selectedApplicationIds, setSelectedApplicationIds] = useState([])
    const [displayedApplications, setDisplayedApplications] = useState(displayedApplicationsPage);

    const [authoredProjects, setAuthoredProjects] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const rejectingApplicationsRequestDialogRef = useRef(null);
    const acceptingApplicationsRequestDialogRef = useRef(null);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const fetchAuthoredProjects = async () => {
        try {
            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let result = await getAuthored(token);

            if (result.outcome == successResult) {
                let projects = await result.payload;

                setAuthoredProjects(projects);

            } else {
                console.log("error fetching authored projects");
            }
        } catch (ex) {
            console.log("error fetching authored projects", ex);
        }
    };

    useEffect(() => {
        fetchAuthoredProjects();
    }, [])

    /* mirroring prop bc we'll fake fetching data after a successful API call with UI changes */
    useEffect(() => {
        setDisplayedApplications(displayedApplicationsPage);
    }, [displayedApplicationsPage]);

    useEffect(() => {
        setSelectedApplicationIds([]);
    }, [displayedApplications])

    useEffect(() => {
        onProjectIdFilterChanged(projectIdFilter);
    }, [projectIdFilter])

    useEffect(() => {
        onSortChanged(sort);
    }, [sort])

    function applicationSelected(applicationId) {
        setSelectedApplicationIds([...selectedApplicationIds, applicationId])
    }

    function applicationDeselected(applicationId) {
        setSelectedApplicationIds(selectedApplicationIds.filter(id => id != applicationId));
    }

    function handleRejectSelectedApplicationsConfirmed() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                for (let applicationId of selectedApplicationIds) {
                    let result = await rejectApplication(applicationId, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        setToggleReload(!toggleReload);
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                }
            }
            catch (ex) {
                console.log("exception", ex);
            } finally {
                rejectingApplicationsRequestDialogRef.current.close();
            }
        })();
    }

    function handleRejectSelectedApplicationsRequested() {
        rejectingApplicationsRequestDialogRef.current.showModal();
    }

    function handleAcceptSelectedApplicationsConfirmed() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let successfullyAcceptedApplicationIds = [];

                for (let applicationId of selectedApplicationIds) {
                    var result = await acceptApplication(applicationId, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        successfullyAcceptedApplicationIds.push(applicationId);
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                }

                setDisplayedApplications(displayedApplications.filter(
                    displayedApplication => !successfullyAcceptedApplicationIds.find(id => id == displayedApplication.id)
                ));
            }
            catch (ex) {
                console.log("exception", ex);
            } finally {
                acceptingApplicationsRequestDialogRef.current.close();
            }
        })();
    }

    function handleAcceptSelectedApplicationsRequested() {
        acceptingApplicationsRequestDialogRef.current.showModal();
    }

    const rejectingApplicationsRequestDialog = (
        <dialog ref={rejectingApplicationsRequestDialogRef}>
            <ConfirmationDialog
                question="Are you sure you want to reject selected applications?"
                description="You won't be able to accept these applications"
                onConfirm={handleRejectSelectedApplicationsConfirmed}
                onCancel={() => rejectingApplicationsRequestDialogRef.current.close()}>
            </ConfirmationDialog>
        </dialog>
    );

    const acceptingApplicationsRequestDialog = (
        <dialog ref={acceptingApplicationsRequestDialogRef}>
            <ConfirmationDialog
                question="Are you sure you want to accept selected applications?"
                description="Contributors who submitted them will become official collaborators on the project"
                onConfirm={handleAcceptSelectedApplicationsConfirmed}
                onCancel={() => acceptingApplicationsRequestDialogRef.current.close()}>
            </ConfirmationDialog>
        </dialog>
    );

    return (
        <>
            {rejectingApplicationsRequestDialog}
            {acceptingApplicationsRequestDialog}

            <div className='relative pb-32'>
                <div className='flex flex-row px-2 mb-12 flex-wrap justify-start space-x-8 items-center'>
                    <div className='w-64'>
                        {
                            !authoredProjects &&
                            <div className='flex items-center gap-x-2 text-gray-600'>
                                Loading projects
                                <ClipLoader size={16}></ClipLoader>
                            </div>
                        }

                        {
                            authoredProjects &&
                            <ProjectFilter
                                projects={authoredProjects}
                                onProjectSelected={(project) => setProjectIdFilter(project ? project.id : undefined)}
                                onProjectDeselected={() => { }}
                                selectedProjectId={projectIdFilter}>
                            </ProjectFilter>
                        }
                    </div>

                    <ApplicationsSorter
                        sort={sort}
                        onSortSelected={setSort}>
                    </ApplicationsSorter>
                </div>

                <ApplicationsTable>
                    <ApplicationsTable.Header>
                        <ApplicationsTable.Header.Cell>Project</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Position</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Date submitted</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Applicant</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Status</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell></ApplicationsTable.Header.Cell>
                    </ApplicationsTable.Header>

                    <ApplicationsTable.Body>
                        {
                            !displayedApplications &&
                            <BeatLoader></BeatLoader>
                        }

                        {
                            displayedApplications &&
                            displayedApplications.items.map(application => <ApplicationsTable.Body.Row selected={selectedApplicationIds.find(id => id == application.id) != undefined}>
                                <ApplicationsTable.Body.Cell>
                                    <Link to={`/${application.projectUrl}`}><p className='underline text-blue-500 hover:text-blue-700 truncate'>{application.project.title}</p></Link>
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    {application.project.positions.find(p => p.id == application.positionId).name}
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    {application.dateSubmitted}
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    <Link to={`/${application.applicantUrl}`}>
                                        <p className='underline text-blue-500 hover:text-blue-700 truncate'>{application.applicant.fullName}</p>
                                    </Link>
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    <SubmittedApplicationStatus></SubmittedApplicationStatus>
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    <form onChange={() => { }}>
                                        <input
                                            className='cursor-pointer'
                                            type='checkbox'
                                            checked={selectedApplicationIds.find(id => id == application.id)}
                                            onChange={() => selectedApplicationIds.find(id => id == application.id) ?
                                                applicationDeselected(application.id) :
                                                applicationSelected(application.id)}>
                                        </input>
                                    </form>
                                </ApplicationsTable.Body.Cell>
                            </ApplicationsTable.Body.Row>)
                        }
                    </ApplicationsTable.Body>

                    <ApplicationsTable.Footer>
                        <ApplicationsTable.Footer.Row>
                            <ApplicationsTable.Footer.Cell>{`Total: ${displayedApplicationsPage.totalItems}`}</ApplicationsTable.Footer.Cell>

                            <ApplicationsTable.Footer.Cell collspan={4}>
                                <div className='flex shrink-0 items-center w-full align-middle justify-center'>
                                    <BorderlessButton
                                        onClick={() => { if (displayedApplicationsPage.previousPage != null) onPageChanged(displayedApplicationsPage.previousPage) }}
                                        disabled={displayedApplicationsPage.previousPage == null}
                                        icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5 8.25 12l7.5-7.5" />
                                        </svg>
                                        }>
                                    </BorderlessButton>

                                    <p>
                                        {`page ${displayedApplicationsPage.page} /  
                                        ${displayedApplicationsPage.nextPage != null ? displayedApplicationsPage.nextPage : displayedApplicationsPage.page}`}
                                    </p>

                                    <BorderlessButton
                                        onClick={() => { if (displayedApplicationsPage.nextPage != null) onPageChanged(displayedApplicationsPage.nextPage) }}
                                        disabled={displayedApplicationsPage.nextPage == null}
                                        icon={<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="m8.25 4.5 7.5 7.5-7.5 7.5" />
                                        </svg>
                                        }>
                                    </BorderlessButton>
                                </div>
                            </ApplicationsTable.Footer.Cell>

                            <ApplicationsTable.Footer.Cell>
                                <p className='w-28 pl-2 text-left'>{`Selected: ${selectedApplicationIds.length}`}</p>
                            </ApplicationsTable.Footer.Cell>
                        </ApplicationsTable.Footer.Row>
                    </ApplicationsTable.Footer>
                </ApplicationsTable>

                <div className='absolute bottom-0 right-0 flex flex-row gap-x-4'>
                    <TertiaryButton
                        text="Cancel"
                        disabled={selectedApplicationIds.length == 0}
                        onClick={() => setSelectedApplicationIds([])}>
                    </TertiaryButton>

                    <TertiaryButton
                        disabled={selectedApplicationIds.length == 0}
                        onClick={handleRejectSelectedApplicationsRequested}
                        text="Reject">
                    </TertiaryButton>

                    <PrimaryButton
                        disabled={selectedApplicationIds.length == 0}
                        onClick={handleAcceptSelectedApplicationsRequested}
                        text="Accept">
                    </PrimaryButton>
                </div>
            </div >
        </>
    )
}

export default ReceivedApplications