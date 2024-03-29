import { deleteAsync, getAsync, postAsync, putAsync } from './ApiService'
import {
    successResult,
    failureResult,
    errorResult
} from './RequestResult'

async function publish(publishModel, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects`, publishModel, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project published successfully!"
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

async function addPositions(projectId, positions, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects/${projectId}/positions`, positions, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Positions added successfully!"
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

async function updateDetails(projectId, title, description, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await putAsync(
            `${apiRootUri}/projects/${projectId}/details`,
            { title: title, description: description },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project details updated successfully!"
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

function AppendQueryString(baseUri, keyword, sort, hat, page = 1) {
    var searchRefinemets = [];

    if (keyword != undefined)
        searchRefinemets["keyword"] = keyword;

    if (sort == "asc") {
        searchRefinemets["sort"] = "byDatePostedAsc";
    } else if (sort == "desc") {
        searchRefinemets["sort"] = "byDatePostedDesc";
    }

    if (hat != undefined) {
        searchRefinemets["hatType"] = hat.type;

        Object.keys(hat.parameters).forEach(k => searchRefinemets[k] = hat.parameters[k]);
    }

    searchRefinemets["page"] = page;

    if (Object.keys(searchRefinemets).length > 0) {
        baseUri += "?";

        for (let parameter in searchRefinemets)
            if (searchRefinemets[parameter] != undefined)
                baseUri += `${parameter}=${encodeURI(searchRefinemets[parameter])}&`

        baseUri = baseUri.slice(0, -1);
    }

    return baseUri;
}

async function discover(keyword, sort, hat, accessToken, page = 1) {
    let projectModels = [];

    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        let projectsUri = `${apiRootUri}/projects`;
        let queryParamsUri = AppendQueryString(projectsUri, keyword, sort, hat, page);

        let fetchProjectsResponse = await getAsync(queryParamsUri, accessToken);

        if (!fetchProjectsResponse.ok) {
            return {
                outcome: failureResult,
                message: "Error fetching projects"
            };
        }

        let discoveredProjectsResult = await fetchProjectsResponse.json();
        let discoveredProjects = discoveredProjectsResult.items;

        let projectAuthors = [];
        let uniqueAuthorUris = [... new Set(discoveredProjects.map(p => p.authorUrl))];

        for (let i = 0; i < uniqueAuthorUris.length; ++i) {
            let fetchAuthorResponse = await getAsync(`${apiRootUri}/${uniqueAuthorUris[i]}`, accessToken);

            if (!fetchAuthorResponse.ok) {
                return {
                    outcome: failureResult,
                    message: "Error fetching project authors",
                };
            }

            let projectAuthor = await fetchAuthorResponse.json();
            projectAuthors = [...projectAuthors, projectAuthor];
        }

        let projectCollaborations = [];

        for (let i = 0; i < discoveredProjects.length; ++i) {
            let fetchCollaborationsResponse = await getAsync(`${apiRootUri}/${discoveredProjects[i].collaborationsUrl}`, accessToken);

            if (!fetchCollaborationsResponse.ok) {
                return {
                    outcome: failureResult,
                    message: "Error fetching project collaborations",
                };
            }

            let collaboration = await fetchCollaborationsResponse.json();
            projectCollaborations = [...projectCollaborations, collaboration];
        }

        for (let i = 0; i < discoveredProjects.length; ++i) {
            let thisProjectsCollaborations = projectCollaborations.filter(c => c.some(c => c.projectId == discoveredProjects[i].id))[0] ?? [];

            for (let j = 0; j < thisProjectsCollaborations.length; ++j) {
                let fetchCollaboratorResponse = await getAsync(`${apiRootUri}/contributors/${thisProjectsCollaborations[j].collaboratorId}`, accessToken);

                if (!fetchCollaboratorResponse.ok) {
                    return {
                        outcome: failureResult,
                        message: "Error fetching project collaborations",
                    };
                }

                let collaborator = await fetchCollaboratorResponse.json();

                thisProjectsCollaborations[j].collaboratorEmail = collaborator.email;
                thisProjectsCollaborations[j].collaboratorName = collaborator.fullName;
            }

            projectModels = [...projectModels,
            {
                ...discoveredProjects[i],
                author: projectAuthors.filter(a => discoveredProjects[i].authorUrl.includes(a.id))[0],
                collaborations: thisProjectsCollaborations
            }]
        }

        return {
            outcome: successResult,
            payload: {
                ...discoveredProjectsResult,
                items: projectModels
            }
        };
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function getById(projectId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        let fetchProjectRequestUri = `${apiRootUri}/projects/${projectId}`;
        let fetchProjectResponse = await getAsync(fetchProjectRequestUri, accessToken);

        if (!fetchProjectResponse.ok) {
            let message = await fetchProjectResponse.text();

            return {
                outcome: failureResult,
                message: message
            };
        }

        let project = await fetchProjectResponse.json();

        let fetchAuthorRequestUri = `${apiRootUri}/${project.authorUrl}`;
        let fetchAuthorResponse = await getAsync(fetchAuthorRequestUri, accessToken);

        if (!fetchAuthorResponse.ok) {
            return {
                outcome: failureResult,
                message: "Error fetching project author"
            };
        }

        let author = await fetchAuthorResponse.json();

        let fetchCollaborationsUri = `${apiRootUri}/${project.collaborationsUrl}`;
        let fetchCollaborationsResponse = await getAsync(fetchCollaborationsUri, accessToken);

        if (!fetchCollaborationsResponse.ok) {
            return {
                outcome: failureResult,
                message: "Error fetching project collaborations"
            };
        }

        let collaborations = await fetchCollaborationsResponse.json();

        for (let c of collaborations) {
            let fetchCollaboratorUri = `${apiRootUri}/${c.collaboratorUrl}`;

            let fetchCollaboratorResponse = await getAsync(fetchCollaboratorUri, accessToken);

            if (!fetchCollaboratorResponse.ok) {
                return {
                    outcome: failureResult,
                    message: "Error fetching project collaborations"
                };
            }

            let collaborator = await fetchCollaboratorResponse.json();
            c.collaborator = collaborator;
        }

        let fetchApplicationsUrl = `${apiRootUri}/${project.applicationsUrl}`;
        let fetchApplicationsResponse = await getAsync(fetchApplicationsUrl, accessToken);

        if (!fetchApplicationsResponse.ok) {
            return {
                outcome: failureResult,
                message: "Error fetching project applications"
            };
        }

        let applications = await fetchApplicationsResponse.json();

        return {
            outcome: successResult,
            payload: {
                ...project,
                author: author,
                collaborations: collaborations,
                applications: applications
            }
        };

    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function getAuthored(accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/projects/authored`;

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            var body = await response.json();

            return {
                outcome: successResult,
                message: "Authored projects successfully retrieved!",
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

async function remove(projectId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/projects/${projectId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project removed successfully!"
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

async function closePosition(projectId, positionId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var response = await putAsync(
            `${apiRootUri}/projects/${projectId}/positions/${positionId}`,
            { open: false },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Position closed successfully!"
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

async function reopenPosition(projectId, positionId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await putAsync(
            `${apiRootUri}/projects/${projectId}/positions/${positionId}`,
            { open: true },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Position reopened successfully!"
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

async function removePosition(projectId, positionId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/projects/${projectId}/positions/${positionId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Position removed successfully!"
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
    publish,
    addPositions,
    discover,
    getById,
    getAuthored,
    updateDetails,
    remove,
    closePosition,
    reopenPosition,
    removePosition
}