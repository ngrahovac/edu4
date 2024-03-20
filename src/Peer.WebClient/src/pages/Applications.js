import React from 'react'
import { useState, useEffect } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import { getSubmittedApplications, getIncomingApplications } from '../services/ApplicationsService'
import SentApplications from '../comps/applications/SentApplications'
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react';
import ReceivedApplications from '../comps/applications/ReceivedApplications'
import { HatSearchParam } from '../comps/search/HatSearchParam'
import { Link, useParams } from 'react-router-dom'
import { Navigate } from 'react-router-dom'

const Applications = () => {
    const applicationType = {
        sent: "sent",
        received: "received"
    };

    const { getAccessTokenSilently } = useAuth0();

    const { selectedApplicationType } = useParams();
    const [sentApplications, setSentApplications] = useState(undefined);
    const [projectIdFilter, setProjectIdFilter] = useState(undefined);
    const [sort, setSort] = useState(undefined);

    const [receivedApplications, setReceivedApplications] = useState(undefined);

    useEffect(() => {
        if (selectedApplicationType == applicationType.sent) {
            getSentApplications();
        } else {
            getReceivedApplications();
        }
    }, [selectedApplicationType, sort, projectIdFilter])
   

    if (selectedApplicationType != applicationType.sent &&
        selectedApplicationType != applicationType.received) {
            return <Navigate to="/404" replace></Navigate>
    }


    function getSentApplications() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getSubmittedApplications(token, projectIdFilter, sort);

                if (result.outcome === successResult) {
                    const sentApplications = result.payload;
                    setSentApplications(sentApplications.items);
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log(ex);
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    function getReceivedApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getIncomingApplications(token, projectIdFilter, sort);

                if (result.outcome === successResult) {
                    const receivedApplications = result.payload;
                    setReceivedApplications(receivedApplications);
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log(ex);
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    return (
        <SingleColumnLayout
            title="Applications"
            description="Manage sent and received project applications">

            <div className='pt-16 w-full flex flex-col gap-y-8'>
                <div className="flex gap-x-4 items-center text-gray-600">
                    <Link to="/applications/sent">
                        <HatSearchParam
                            selected={selectedApplicationType == "sent"}>
                            Sent
                        </HatSearchParam>
                    </Link>

                    <Link to="/applications/received">
                        <HatSearchParam
                            selected={selectedApplicationType == "received"}>
                            Received
                        </HatSearchParam>
                    </Link>
                </div>

                {/* applications */}
                <div>
                    {
                        selectedApplicationType == applicationType.sent &&
                        sentApplications != undefined &&
                        <SentApplications
                            applications={sentApplications}
                            onProjectIdFilterChanged={(projectId) => { setProjectIdFilter(projectId) }}
                            onSortChanged={(sort) => { setSort(sort ? sort : undefined) }}></SentApplications>
                    }
                    {
                        selectedApplicationType == applicationType.received &&
                        receivedApplications != undefined &&
                        <ReceivedApplications
                            applications={receivedApplications}
                            onProjectIdFilterChanged={(projectId) => { setProjectIdFilter(projectId) }}
                            onSortChanged={(sort) => { setSort(sort ? sort : undefined) }}>
                        </ReceivedApplications>
                    }
                </div>
            </div>
        </SingleColumnLayout>
    )
}

export default Applications