import React, { useEffect, useRef, useState } from 'react'
import { getById } from '../../services/ProjectsService';
import { useAuth0 } from '@auth0/auth0-react';
import ReceivedApplication from './ReceivedApplication';
import PrimaryButton from '../buttons/PrimaryButton';
import { acceptApplication, rejectApplication, revokeApplication } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
import { getContributor } from '../../services/UsersService';
import { getAuthored } from '../../services/ProjectsService';
import DangerButton from '../buttons/DangerButton';
import { Fragment } from 'react';
import ProjectFilter from './ProjectFilter';
import ApplicationsSorter from './SentApplicationsSorter';
import { BeatLoader } from 'react-spinners';
import ConfirmationDialog from '../shared/ConfirmationDialog';


const ReceivedApplications = (props) => {
    const {
        applications,
        onProjectIdFilterChanged,
        onSortChanged
    } = props;

    const [displayedApplicationsProjects, setDisplayedApplicationsProjects] = useState(undefined);
    const [applicants, setApplicants] = useState([]);
    const [selectedApplicationIds, setSelectedApplicationIds] = useState([])
    const [displayedApplications, setDisplayedApplications] = useState(applications);

    const [authoredProjects, setAuthoredProjects] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const rejectingApplicationsRequestDialogRef = useRef(null);
    const acceptingApplicationsRequestDialogRef = useRef(null);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const [loading, setLoading] = useState(true);

    const fetchProjectsForDisplayedApplications = async () => {
        setLoading(true);

        try {
            let fetchedProjects = [];

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            for (let application of displayedApplications) {
                try {
                    let result = await getById(application.projectId, token);

                    if (result.outcome == successResult) {
                        let project = await result.payload;

                        fetchedProjects.push(project);
                    } else {
                        console.log("error fetching one of the projects for received applications");
                    }
                } catch (ex) {
                    console.log(ex);
                }
            };

            setDisplayedApplicationsProjects(fetchedProjects);
        } catch (ex) {
            console.log("error fetching projects for displayed applications");
        } finally {
            setLoading(false);
        }
    }

    const fetchApplicantsForDisplayedApplications = async () => {
        setLoading(true);

        try {
            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let fetchedApplicants = [];

            for (let application of displayedApplications) {
                try {
                    let result = await getContributor(token, application.applicantUrl);

                    if (result.outcome == successResult) {
                        let applicant = await result.payload;

                        fetchedApplicants.push(applicant);
                    } else {
                        console.log("error fetching an applicant")
                    }
                } catch (ex) {
                    console.log(ex);
                }
            };

            setApplicants(fetchedApplicants);
        } catch (ex) {
            console.log("error fetching all applicants for received applications", ex);
        } finally {
            setLoading(false);
        }
    };

    const fetchAuthoredProjects = async () => {
        setLoading(true);

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
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAuthoredProjects();
    }, [])

    /* mirroring prop bc we'll fake fetching data after a successful API call with UI changes */
    useEffect(() => {
        setDisplayedApplications(applications);
    }, [applications]);

    useEffect(() => {
        fetchProjectsForDisplayedApplications();
        fetchApplicantsForDisplayedApplications();
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

                let successfullyRejectedApplicationIds = [];

                for (let applicationId of selectedApplicationIds) {
                    let result = await rejectApplication(applicationId, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        successfullyRejectedApplicationIds.push(applicationId);
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                }

                setDisplayedApplications(displayedApplications.filter(
                    displayedApplication => !successfullyRejectedApplicationIds.find(
                        id => id == displayedApplication.id
                    )
                ));
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

    if (loading) {
        return <BeatLoader></BeatLoader>
    }

    return (
        displayedApplicationsProjects &&
        applicants &&
        authoredProjects &&
        <>
            {rejectingApplicationsRequestDialog}
            {acceptingApplicationsRequestDialog}

            <div className='relative pb-32'>
                <div className='flex flex-row px-2 mb-12 flex-wrap justify-start space-x-8'>
                    <ProjectFilter
                        projects={authoredProjects}
                        onProjectSelected={(project) => setProjectIdFilter(project ? project.id : undefined)}
                        onProjectDeselected={() => { }}
                        selectedProjectId={projectIdFilter}>
                    </ProjectFilter>

                    <ApplicationsSorter sort={sort} onSortSelected={setSort}>

                    </ApplicationsSorter>
                </div>

                <div className='overflow-x-auto'>
                    <table className='text-left w-full table-fixed'>
                        <thead>
                            <tr className=''>
                                <th className='py-4 px-2 pl-4 w-1/4 truncate'>Project</th>
                                <th className='py-4 px-2 w-1/4 truncate'>Position</th>
                                <th className='py-4 px-2 truncate'>Date submitted</th>
                                <th className='py-4 px-2 truncate'>Applicant</th>
                                <th className='py-4 px-2 truncate'>Status</th>
                                <th className='py-4 px-2 pr-4 truncate'></th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                displayedApplications.map(a => <Fragment key={a.id}>
                                    <ReceivedApplication
                                        application={a}
                                        projectTitle={displayedApplicationsProjects.find(p => p.id == a.projectId) ? displayedApplicationsProjects.find(p => p.id == a.projectId).title : "nema"}
                                        positionName={displayedApplicationsProjects.find(p => p.id == a.projectId) ? displayedApplicationsProjects.find(p => p.id == a.projectId).positions.find(p => p.id == a.positionId).name : "nema"}
                                        applicantName={applicants.find(i => i.id == a.applicantId) ? applicants.find(i => i.id == a.applicantId).fullName : "nema"}
                                        onApplicationSelected={applicationSelected}
                                        onApplicationDeselected={applicationDeselected}>
                                    </ReceivedApplication>
                                </Fragment>)
                            }
                        </tbody>
                        <tfoot>
                            <tr>
                                <td className='text-left pl-4 h-12 uppercase tracking-wide'>
                                    {
                                        selectedApplicationIds.length > 0 &&
                                        <p>{`Selected: ${selectedApplicationIds.length}`}</p>
                                    }
                                </td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>

                <div className='absolute bottom-0 right-0 flex flex-row space-x-2'>
                    <DangerButton
                        disabled={selectedApplicationIds.length == 0}
                        onClick={handleRejectSelectedApplicationsRequested}
                        text="Reject"></DangerButton>

                    <PrimaryButton
                        disabled={selectedApplicationIds.length == 0}
                        onClick={handleAcceptSelectedApplicationsRequested}
                        text="Accept"></PrimaryButton>
                </div>
            </div>
        </>
    )
}

export default ReceivedApplications