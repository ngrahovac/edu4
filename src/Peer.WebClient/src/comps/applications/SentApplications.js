import React, { Fragment, useEffect, useState } from 'react'
import { getById } from '../../services/ProjectsService';
import { useAuth0 } from '@auth0/auth0-react';
import SentApplication from './SentApplication';
import PrimaryButton from '../buttons/PrimaryButton';
import { revokeApplication, getSubmittedApplicationsProjectIds } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
import SentApplicationsProjectSelector from './SentApplicationsProjectSelector';
import SentApplicationsSorter from './SentApplicationsSorter';

const SentApplications = (props) => {

    const {
        applications,
        onProjectIdFilterChanged,
        onSortChanged
    } = props;

    const [displayedApplicationsProjects, setDisplayedApplicationsProjects] = useState(undefined);
    const [selectedApplicationIds, setSelectedApplicationIds] = useState([])
    const [displayedApplications, setDisplayedApplications] = useState(applications);

    const [submittedApplicationsProjects, setSubmittedApplicationsProjects] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const fetchProjectsForDisplayedApplications = async () => {
        try {
            var fetchedProjects = [];

            displayedApplications.forEach(async application => {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getById(application.projectId, token);

                if (result.outcome == successResult) {
                    let project = await result.payload;
                    fetchedProjects.push(project);
                } else {
                    console.log("error fetching a single project")
                }
            });

            setDisplayedApplicationsProjects(fetchedProjects);
        } catch (ex) {
            console.log("error fetching one project for currently displayed applications", ex);
        }
    };

    const fetchProjectsUserAppliedFor = async () => {
        try {
            let fetchedProjects = [];

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let result = await getSubmittedApplicationsProjectIds(token);

            if (result.outcome == successResult) {
                let projectIds = await result.payload;

                projectIds.map(async id => {
                    let result = await getById(id, token);

                    if (result.outcome == successResult) {
                        let project = await result.payload;

                        fetchedProjects.push(project);
                    } else {
                        console.log("error fetching one of the projects user applied to")
                    }
                });
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
        fetchProjectsForDisplayedApplications();
        fetchProjectsUserAppliedFor();
        setSelectedApplicationIds([]);
    }, [displayedApplications]);

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

    function onRevokeSelectedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                selectedApplicationIds.forEach(async application => {
                    var result = await revokeApplication(application.id, token);

                    if (result.outcome === successResult) {
                        setDisplayedApplications(displayedApplications.filter(a => a.id != application.id));
                        setSelectedApplicationIds(selectedApplicationIds.filter(a => a.id != application.id));
                    }
                    else if (result.outcome === failureResult) {
                        console.log("neuspjesan status code");
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    } else if (result.outcome === errorResult) {
                        console.log("nesto je do mreze", result);
                        // document.getElementById('user-action-fail-toast').show();
                        // setTimeout(() => {
                        //     document.getElementById('user-action-fail-toast').close();
                        // }, 3000);
                    }
                });
            }
            catch (ex) {
                console.log(ex);
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    return (
        displayedApplicationsProjects &&
        submittedApplicationsProjects &&
        <div className='relative pb-32'>
            <div className='flex flex-row px-2 mb-12 flex-wrap justify-start space-x-8'>
                <SentApplicationsProjectSelector
                    projects={submittedApplicationsProjects}
                    onProjectSelected={(project) => setProjectIdFilter(project ? project.id : undefined)}
                    onProjectDeselected={() => setProjectIdFilter(undefined)}>
                </SentApplicationsProjectSelector>

                <SentApplicationsSorter onSortSelected={(sort) => setSort(sort)}>
                </SentApplicationsSorter>
            </div>


            <div className='overflow-x-auto'>
                <table className='text-left w-full'>
                    <thead>
                        <tr className=''>
                            <th className='py-4 px-2 pl-4 w-1/3 truncate'>Project</th>
                            <th className='py-4 px-2 w-1/4 truncate'>Position</th>
                            <th className='py-4 px-2 truncate'>Date submitted</th>
                            <th className='py-4 px-2 truncate'>Status</th>
                            <th className='py-4 px-2 pr-4 truncate'></th>
                        </tr>

                    </thead>
                    <tbody>
                        {
                            displayedApplications.map(a => <Fragment key={a.id}>
                                <SentApplication
                                    application={a}
                                    projectTitle={displayedApplicationsProjects.find(p => p.id == a.projectId) ? displayedApplicationsProjects.find(p => p.id == a.projectId).title : "nema"}
                                    positionName={displayedApplicationsProjects.find(p => p.id == a.projectId) ? displayedApplicationsProjects.find(p => p.id == a.projectId).positions.find(p => p.id == a.positionId).name : "nema"}
                                    onApplicationSelected={applicationSelected}
                                    onApplicationDeselected={applicationDeselected}>
                                </SentApplication>
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
                        </tr>
                    </tfoot>
                </table>
            </div>

            <div className='absolute bottom-0 right-0 flex flex-row space-x-8'>
                <PrimaryButton
                    disabled={selectedApplicationIds.length == 0}
                    onClick={onRevokeSelectedApplications}
                    text="Revoke"></PrimaryButton>
            </div>
        </div>
    )
}

export default SentApplications