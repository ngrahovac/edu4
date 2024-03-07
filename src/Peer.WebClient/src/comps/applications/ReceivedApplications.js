import React, { useEffect, useRef, useState } from 'react'
import { useAuth0 } from '@auth0/auth0-react';
import ReceivedApplication from './ReceivedApplication';
import PrimaryButton from '../buttons/PrimaryButton';
import { acceptApplication, rejectApplication } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
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

    const [toggleReload, setToggleReload] = useState(false);

    const [selectedApplicationIds, setSelectedApplicationIds] = useState([])
    const [displayedApplications, setDisplayedApplications] = useState(applications);

    const [authoredProjects, setAuthoredProjects] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const rejectingApplicationsRequestDialogRef = useRef(null);
    const acceptingApplicationsRequestDialogRef = useRef(null);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const [loading, setLoading] = useState(true);

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

    if (loading) {
        return <BeatLoader></BeatLoader>
    }

    return (
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
                            <tr className='border-b-2 border-gray-200'>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'>Project</th>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'>Position</th>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'>Date submitted</th>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'>Applicant</th>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'>Status</th>
                                <th className='w-1/4 truncate font-medium text-sm p-4 text-gray-400 uppercase'></th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                displayedApplications.map(a => <Fragment key={a.id}>
                                    <ReceivedApplication
                                        application={a}
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