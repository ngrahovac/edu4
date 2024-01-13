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
import TabulatedMenu from '../comps/nav/TabulatedMenu'

const Applications = () => {
    const applicationType = {
        sent: "Sent",
        received: "Received"
    };

    const { getAccessTokenSilently } = useAuth0();

    const [selectedApplicationType, setSelectedApplicationType] = useState(applicationType.sent);

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


    function getSentApplications() {
        (async () => {
            try {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getSubmittedApplications(token, projectIdFilter, sort);

                if (result.outcome === successResult) {
                    const sentApplications = result.payload;
                    setSentApplications(sentApplications);
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

            <div className='mt-16 w-full'>
                <TabulatedMenu
                    items={["Sent", "Received"]}
                    onSelectionChange={(item) =>
                        setSelectedApplicationType(item)}>
                </TabulatedMenu>

                {/* applications */}
                <div className='mt-16'>
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