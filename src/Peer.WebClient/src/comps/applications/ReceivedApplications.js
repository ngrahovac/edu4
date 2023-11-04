import React, { useEffect, useState } from 'react'
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

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    const [loading, setLoading] = useState(true);

    const fetchProjectsForDisplayedApplications = async () => {
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

        }
    }

    const fetchApplicantsForDisplayedApplications = async () => {
        try {
            let fetchedApplicants = [];

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

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
        }
    };

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

    useEffect(() => {
        setLoading(true);
        fetchProjectsForDisplayedApplications();
        fetchApplicantsForDisplayedApplications();
        setSelectedApplicationIds([]);
        setLoading(false);
    }, [displayedApplications])

    /* mirroring prop bc we'll fake fetching data after a successful API call with UI changes */
    useEffect(() => {
        setDisplayedApplications(applications);
    }, [applications]);

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

    function onRejectSelectedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                selectedApplicationIds.forEach(async application => {
                    var result = await rejectApplication(application.id, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        setDisplayedApplications(displayedApplications.filter(a => a.id != application.id));
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                });
            }
            catch (ex) {
                console.log("exception", ex);
            }
        })();
    }

    function onAcceptSelectedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                selectedApplicationIds.forEach(async application => {
                    var result = await acceptApplication(application.id, token);

                    if (result.outcome === successResult) {
                        console.log("success");
                        setDisplayedApplications(displayedApplications.filter(a => a.id != application.id));
                        setSelectedApplicationIds(selectedApplicationIds.filter(a => a.id != application.id));
                    }
                    else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                });
            }
            catch (ex) {
                console.log("exception", ex);
            }
        })();
    }

    return (
        displayedApplicationsProjects &&
        applicants &&
        displayedApplications &&
        authoredProjects &&
        <div className='relative pb-32'>
            <div className='flex flex-row px-2 mb-12 flex-wrap justify-start space-x-8'>
                <ProjectFilter
                    projects={authoredProjects}
                    onProjectSelected={(project) => setProjectIdFilter(project ? project.id : undefined)}
                    onProjectDeselected={() => setProjectIdFilter(undefined)}>
                </ProjectFilter>

                <ApplicationsSorter onSortSelected={setSort}>

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
                    onClick={onRejectSelectedApplications}
                    text="Reject"></DangerButton>

                <PrimaryButton
                    disabled={selectedApplicationIds.length == 0}
                    onClick={onAcceptSelectedApplications}
                    text="Accept"></PrimaryButton>
            </div>
        </div>
    )
}

export default ReceivedApplications