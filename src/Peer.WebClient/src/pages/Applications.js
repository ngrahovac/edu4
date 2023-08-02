import React from 'react'
import { useState, useEffect } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import { getSubmittedApplications } from '../services/ApplicationsService'
import SentApplications from '../comps/applications/SentApplications'
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react';


const Applications = () => {

    const [displayedApplications, setDisplayedApplications] = useState("Sent");
    const [sentApplications, setSentApplications] = useState(undefined);

    const { getAccessTokenWithPopup } = useAuth0();

    function getSentApplications() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await getSubmittedApplications(token);

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

    useEffect(() => {
        if (displayedApplications == "Sent") {
            getSentApplications();
        }
    }, [displayedApplications])


    return (
        <SingleColumnLayout
            title="Applications"
            description="Manage sent and received project applications">

            <div className='mt-16'>
                {/* sent/received menu */}
                <div className='flex flex-row space-x-8 border-b-2 pb-2'>
                    <p
                        onClick={() => setDisplayedApplications("Sent")}
                        className={`font-semibold ${displayedApplications == "Sent" ? "text-blue-500" : "text-gray-700"} cursor-pointer hover:text-blue-300`}>
                        Sent
                    </p>
                    <p
                        onClick={() => setDisplayedApplications("Received")}
                        className={`font-semibold ${displayedApplications == "Received" ? "text-blue-500" : "text-gray-700"} cursor-pointer hover:text-blue-300`}>
                        Received
                    </p>
                </div>

                {/* sent applications */}
                <div className='mt-16'>
                {
                    displayedApplications == "Sent" &&
                    sentApplications != undefined &&
                    <SentApplications applications={sentApplications}></SentApplications>
                }
                </div>
            </div>
        </SingleColumnLayout>
    )
}

export default Applications