import React, { useEffect, useState, useRef } from 'react'
import { getById } from '../../services/ProjectsService';
import { useAuth0 } from '@auth0/auth0-react';
import PrimaryButton from '../buttons/PrimaryButton';
import { revokeApplication, getSubmittedApplicationsProjectIds } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
import ProjectFilter from './ProjectFilter'
import ApplicationsSorter from './SentApplicationsSorter';
import { BeatLoader, ClipLoader } from 'react-spinners';
import ConfirmationDialog from '../shared/ConfirmationDialog';
import SubmittedApplicationStatus from './SubmittedApplicationStatus';
import { Link } from 'react-router-dom';
import TertiaryButton from '../buttons/TertiaryButton';
import ApplicationsTable from '../table2/ApplicationsTable';

const SentApplications = (props) => {

    const {
        applications,
        onProjectIdFilterChanged,
        onSortChanged
    } = props;

    const [selectedApplicationIds, setSelectedApplicationIds] = useState([])
    const [displayedApplications, setDisplayedApplications] = useState(applications);

    const [submittedApplicationsProjects, setSubmittedApplicationsProjects] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const revokingApplicationRequestDialogRef = useRef(null);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const fetchProjectsUserAppliedFor = async () => {
        try {
            let fetchedProjects = [];

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let result = await getSubmittedApplicationsProjectIds(token);

            if (result.outcome == successResult) {
                let projectIds = await result.payload;

                for (let projectId of projectIds) {
                    let result = await getById(projectId, token);

                    if (result.outcome == successResult) {
                        let project = await result.payload;

                        fetchedProjects.push(project);
                    } else {
                        console.log("error fetching one of the projects user applied to")
                    }
                }
            } else {
                console.log("error fetching all projects user applied to");
            }

            setSubmittedApplicationsProjects(fetchedProjects);
        } catch (ex) {
            console.log("error fetching all projects user applied to", ex);
        }
    };

    /* mirroring prop bc we'll fake fetching data after a successful API call with UI changes */
    useEffect(() => {
        setDisplayedApplications(applications);
    }, [applications]);

    useEffect(() => {
        setSelectedApplicationIds([]);
    }, [displayedApplications]);

    useEffect(() => {
        fetchProjectsUserAppliedFor();
    }, [])

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

    function handleRevokeApplicationsRequested() {
        revokingApplicationRequestDialogRef.current.showModal();
    }

    function handleRevokeApplicationConfirmed() {
        (async () => {
            try {

                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let successfullyRevokedApplicationIds = [];

                for (let applicationId of selectedApplicationIds) {
                    let result = await revokeApplication(applicationId, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        successfullyRevokedApplicationIds.push(applicationId);
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error")
                    }
                }

                setDisplayedApplications(displayedApplications.filter(
                    displayedApplication => !successfullyRevokedApplicationIds.find(
                        id => id == displayedApplication.id)));
            }
            catch (ex) {
                console.log("exception", ex);
            } finally {
                revokingApplicationRequestDialogRef.current.close();
            }
        })();
    }

    const revokingApplicationRequestDialog = (
        <dialog ref={revokingApplicationRequestDialogRef}>
            <ConfirmationDialog
                question="Are you sure you want to revoke selected submitted applications?"
                description="The project author will no longer be able to see your application"
                onConfirm={handleRevokeApplicationConfirmed}
                onCancel={() => revokingApplicationRequestDialogRef.current.close()}>
            </ConfirmationDialog>
        </dialog>
    );


    return (
        <>
            {revokingApplicationRequestDialog}

            <div className='relative pb-32'>
                <div className='flex flex-row px-2 mb-12 flex-wrap justify-start gap-x-8 items-center'>
                    <div className='w-64'>
                        {
                            !submittedApplicationsProjects &&
                            <div className='flex items-center gap-x-2 text-gray-600'>
                                Loading projects
                                <ClipLoader size={16}></ClipLoader>
                            </div>
                        }

                        {
                            submittedApplicationsProjects &&
                            <ProjectFilter
                                projects={submittedApplicationsProjects}
                                selectedProjectId={projectIdFilter}
                                onProjectSelected={(project) => setProjectIdFilter(project ? project.id : undefined)}
                                onProjectDeselected={() => { }}>
                            </ProjectFilter>
                        }
                    </div>

                    <ApplicationsSorter
                        sort={sort}
                        onSortSelected={(sort) => setSort(sort)}>
                    </ApplicationsSorter>
                </div>

                <ApplicationsTable>
                    <ApplicationsTable.Header>
                        <ApplicationsTable.Header.Cell>Project</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Position</ApplicationsTable.Header.Cell>
                        <ApplicationsTable.Header.Cell>Date submitted</ApplicationsTable.Header.Cell>
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
                            displayedApplications.map(application => <ApplicationsTable.Body.Row selected={selectedApplicationIds.find(id => id == application.id) != undefined}>
                                <ApplicationsTable.Body.Cell>
                                    <div className='underline text-blue-500 truncate'>
                                        <Link to={`/${application.projectUrl}`}>
                                            {application.project.title}
                                        </Link>
                                    </div>
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    {application.project.positions.find(p => p.id == application.positionId).name}
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    {application.dateSubmitted}
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    <SubmittedApplicationStatus />
                                </ApplicationsTable.Body.Cell>

                                <ApplicationsTable.Body.Cell>
                                    <form onChange={() => { }}>
                                        <input
                                            className='cursor-pointer'
                                            type='checkbox'
                                            checked={selectedApplicationIds.find(id => id == application.id)}
                                            onChange={() => selectedApplicationIds.find(id => id == application.id) ?
                                                applicationDeselected(application.id) :
                                                applicationSelected(application.id)}></input>
                                    </form>
                                </ApplicationsTable.Body.Cell>
                            </ApplicationsTable.Body.Row>)
                        }
                    </ApplicationsTable.Body>

                    <ApplicationsTable.Footer selectedCount={selectedApplicationIds.length}></ApplicationsTable.Footer>
                </ApplicationsTable>

                <div className='absolute bottom-0 right-0 flex flex-row gap-x-4'>
                    <TertiaryButton
                        text="Cancel"
                        disabled={selectedApplicationIds.length == 0}
                        onClick={() => setSelectedApplicationIds([])}>
                    </TertiaryButton>

                    <PrimaryButton
                        disabled={selectedApplicationIds.length == 0}
                        onClick={handleRevokeApplicationsRequested}
                        text="Revoke">
                    </PrimaryButton>
                </div>
            </div >
        </>
    )
}

export default SentApplications