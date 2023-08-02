import React, { useEffect, useState } from 'react'
import { getById } from '../../services/ProjectsService';
import { useAuth0 } from '@auth0/auth0-react';
import { Link } from 'react-router-dom';
import ReceivedApplication from './ReceivedApplication';
import PrimaryButton from '../buttons/PrimaryButton';
import { acceptApplication, rejectApplication, revokeApplication } from '../../services/ApplicationsService';
import { successResult, errorResult, failureResult } from '../../services/RequestResult';
import { getContributor } from '../../services/UsersService';
import DangerButton from '../buttons/DangerButton';



const ReceivedApplications = (props) => {
    const {
        applications
    } = props;

    const [projects, setProjects] = useState([]);
    const [applicants, setApplicants] = useState([]);
    const [selectedApplications, setSelectedApplications] = useState([])
    const [allApplications, setAllApplications] = useState(applications);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    function fetchProjects() {
        (async () => {
            applications.forEach(async application => {
                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await getById(application.projectId, token);

                    if (result.outcome == successResult) {
                        let project = await result.payload;

                        setProjects([...projects, project]);
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });
        })();
    }

    function fetchApplicants() {
        (async () => {
            applications.forEach(async application => {
                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await getContributor(token, application.applicantUrl);

                    if (result.outcome == successResult) {
                        let applicant = await result.payload;

                        setApplicants([...applicants, applicant]);
                    }
                } catch (ex) {
                    console.log(ex);
                }
            });
        })();
    }

    useEffect(() => {
        fetchProjects();
        fetchApplicants();
    }, [])


    function applicationSelected(applicationId) {
        setSelectedApplications([...selectedApplications, applications.find(a => a.id == applicationId)])
    }

    function applicationDeselected(applicationId) {
        setSelectedApplications(selectedApplications.filter(a => a.id != applicationId));
    }

    function onRejectSelectedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                selectedApplications.forEach(async application => {
                    var result = await rejectApplication(application.id, token);

                    if (result.outcome === successResult) {
                        setAllApplications(allApplications.filter(a => a.id != application.id));
                        setSelectedApplications(selectedApplications.filter(a => a.id != application.id));
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

    function onAcceptSelectedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                selectedApplications.forEach(async application => {
                    var result = await acceptApplication(application.id, token);

                    if (result.outcome === successResult) {
                        setAllApplications(allApplications.filter(a => a.id != application.id));
                        setSelectedApplications(selectedApplications.filter(a => a.id != application.id));
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
        projects.length > 0 &&
        <div className='relative pb-32'>
            <table className='text-left w-full table-fixed'>
                <thead>
                    <tr className=''>
                        <th className='py-4 px-2 pl-4 w-1/4'>Project</th>
                        <th className='py-4 px-2 w-1/4'>Position</th>
                        <th className='py-4 px-2'>Date submitted</th>
                        <th className='py-4 px-2'>Applicant</th>
                        <th className='py-4 px-2'>Status</th>
                        <th className='py-4 px-2 pr-4'></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        applicants.length > 0 &&
                        projects.length > 0 &&
                        allApplications.map(a => <>
                            <ReceivedApplication
                                application={a}
                                projectTitle={projects.find(p => p.id == a.projectId).title}
                                positionName={projects.find(p => p.id == a.projectId).positions.find(p => p.id == a.positionId).name}
                                applicantName={applicants.find(i => i.id == a.applicantId).fullName}
                                onApplicationSelected={applicationSelected}
                                onApplicationDeselected={applicationDeselected}>
                            </ReceivedApplication>
                        </>)
                    }
                </tbody>
                <tfoot>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td className='text-center h-12 uppercase tracking-wide'>
                            {
                                selectedApplications.length > 0 &&
                                <p>{`Selected: ${selectedApplications.length}`}</p>
                            }
                        </td>
                    </tr>
                </tfoot>
            </table>

            <div className='absolute bottom-0 right-0 flex flex-row space-x-2'>
                <DangerButton
                    disabled={selectedApplications.length == 0}
                    onClick={onRejectSelectedApplications}
                    text="Reject"></DangerButton>

                <PrimaryButton
                    disabled={selectedApplications.length == 0}
                    onClick={onAcceptSelectedApplications}
                    text="Accept"></PrimaryButton>
            </div>
        </div>
    )
}

export default ReceivedApplications