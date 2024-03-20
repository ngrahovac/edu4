import { deleteAsync, getAsync, postAsync, putAsync } from './ApiService'
import {
    successResult,
    failureResult,
    errorResult
} from './RequestResult'

async function submitApplication(projectId, positionId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var payload = {
            projectId: projectId,
            positionId: positionId
        }
        var response = await postAsync(`${apiRootUri}/applications`, payload, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application submitted successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function getSubmittedApplications(accessToken, projectId, sort, page) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/applications/sent`;

        var queryParams = [];

        if (projectId != undefined)
            queryParams["projectId"] = projectId;

        if (sort != undefined)
            queryParams["sort"] = sort;

        if (page != undefined)
            queryParams["page"] = page;

        if (Object.keys(queryParams).length > 0) {
            requestUri += "?";

            for (let parameter in queryParams)
                if (queryParams[parameter] != undefined)
                    requestUri += `${parameter}=${encodeURI(queryParams[parameter])}&`

            requestUri = requestUri.slice(0, -1);
        }

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            let applications = await response.json();

            let uniqueProjectUrls = new Set(applications.items.map(a => a.projectUrl));
            let projects = [];

            for (let projectUrl of uniqueProjectUrls) {
                let fetchProjectUrl = `${apiRootUri}/${projectUrl}`;
                let fetchProjectResponse = await getAsync(fetchProjectUrl, accessToken);

                if (!fetchProjectResponse.ok) {
                    return {
                        outcome: failureResult,
                        message: "Error fetching project"
                    };
                }

                let project = await fetchProjectResponse.json();
                projects.push(project);
            }

            for (let application of applications.items) {
                application.project = projects.find(p => p.id == application.projectId);
            }

            return {
                outcome: successResult,
                message: "Submitted applications retrieved successfully!",
                payload: applications
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function getIncomingApplications(accessToken, projectId, sort) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/applications/received`;

        var queryParams = [];

        if (projectId != undefined)
            queryParams["projectId"] = projectId;

        if (sort != undefined)
            queryParams["sort"] = sort;

        if (Object.keys(queryParams).length > 0) {
            requestUri += "?";

            for (let parameter in queryParams)
                if (queryParams[parameter] != undefined)
                    requestUri += `${parameter}=${encodeURI(queryParams[parameter])}&`

            requestUri = requestUri.slice(0, -1);
        }

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            let applications = await response.json();
            let uniqueApplicantUrls = new Set(applications.map(a => a.applicantUrl));
            let applicants = [];

            for (let applicantUrl of uniqueApplicantUrls) {
                let fetchApplicantUri = `${apiRootUri}/${applicantUrl}`;
                let fetchApplicantResponse = await getAsync(fetchApplicantUri, accessToken);

                if (!fetchApplicantResponse.ok) {
                    return {
                        outcome: failureResult,
                        message: "Error fetching applicant"
                    };
                }

                let applicant = await fetchApplicantResponse.json();
                applicants.push(applicant);
            }

            for (let application of applications) {
                application.applicant = applicants.find(a => a.id == application.applicantId);
            }

            let uniqueProjectUrls = new Set(applications.map(a => a.projectUrl));
            let projects = [];

            for (let projectUrl of uniqueProjectUrls) {
                let fetchProjectUrl = `${apiRootUri}/${projectUrl}`;
                let fetchProjectResponse = await getAsync(fetchProjectUrl, accessToken);

                if (!fetchProjectResponse.ok) {
                    return {
                        outcome: failureResult,
                        message: "Error fetching project"
                    };
                }

                let project = await fetchProjectResponse.json();
                projects.push(project);
            }

            for (let application of applications) {
                application.project = projects.find(p => p.id == application.projectId);
            }

            return {
                outcome: successResult,
                message: "Received applications retrieved successfully!",
                payload: applications
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function revokeApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/applications/${applicationId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application revoked successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function rejectApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/applications/${applicationId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application rejected successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function acceptApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await putAsync(
            `${apiRootUri}/applications/${applicationId}`,
            {
                status: "Accepted"
            },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application accepted successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function getSubmittedApplicationsProjectIds(accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var response = await getAsync(`${apiRootUri}/applications/sent/projects`, accessToken);

        if (response.ok) {
            let body = await response.json();

            return {
                outcome: successResult,
                message: "Submitted applications project ids retrieved successfully!",
                payload: body
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

export {
    submitApplication,
    getSubmittedApplications,
    getIncomingApplications,
    revokeApplication,
    rejectApplication,
    acceptApplication,
    getSubmittedApplicationsProjectIds
}
