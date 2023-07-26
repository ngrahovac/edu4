import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import RefineButton from '../comps/discover/RefineButton';
import SelectedDiscoveryParameter from '../comps/discover/SearchFilter';
import SelectedDiscoveryParameters from '../comps/discover/SearchFilters';
import { useState, useEffect } from 'react';
import ProjectCard from '../comps/discover/ProjectCard';
import RecommendedProjectCard from '../comps/discover/RecommendedProjectCard';
import DiscoveryParametersSidebar from '../comps/discover/DiscoveryRefinementSidebar';
import { discover } from '../services/ProjectsService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { useAuth0 } from '@auth0/auth0-react';
import { me } from '../services/UsersService'

const Discover = () => {
    const [projects, setProjects] = useState([
    ]);

    const [ownHats, setOwnHats] = useState([]);
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [discoveryRefinementSidebarVisibility, setDiscoveryParametersSidebarVisibility] = useState(false);

    const { getAccessTokenWithPopup, getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await me(token);

                if (result.outcome === successResult) {
                    setOwnHats(result.payload.hats);
                } else {
                    console.log("error fetching users hats");
                }
            } catch (ex) {
                console.log(ex);
            }
        })();
    }, [])

    function onDiscoveryRefinementChanged() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await discover(keyword, sort, hat, token);

                if (result.outcome === successResult) {
                    var projects = result.payload;
                    setProjects(projects);
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
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
        onDiscoveryRefinementChanged();
    }, [keyword, sort, hat])

    function updateDiscoveryParameters(keyword, sort, hat) {
        setKeyword(keyword);
        setHat(hat);
        setSort(sort);
    }

    function showDiscoveryParametersSidebar() {
        setDiscoveryParametersSidebarVisibility(true);
    }

    function hideDiscoveryParametersSidebar() {
        setDiscoveryParametersSidebarVisibility(false);
    }

    return (
        <SingleColumnLayout
            title="Discover projects"
            description="Something encouraging here">

            {/* refine button and selected discovery parameters */}
            <div className='flex flex-col mt-16'>
                <RefineButton onClick={showDiscoveryParametersSidebar}></RefineButton>

                {/* selected discovery parameters */}
                <SelectedDiscoveryParameters>
                    {
                        keyword != undefined &&
                        <SelectedDiscoveryParameter
                            value={`keyword: ${keyword}`}
                            onRemoved={() => setKeyword(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                    {
                        sort != undefined &&
                        <SelectedDiscoveryParameter
                            value={`sort: ${sort == "asc" ? "oldest first" : "newest first"}`}
                            onRemoved={() => setSort(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                    {
                        hat != undefined &&
                        <SelectedDiscoveryParameter
                            value={`looking for: a ${hat.type.toLowerCase()} like me`}
                            onRemoved={() => setHat(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                </SelectedDiscoveryParameters>
            </div>

            { /* discovery results */}
            <div className='mt-16'>
                {
                    projects.length > 0 &&
                    <div className='flex flex-col space-y-8'>
                        {
                            projects.map(p => <>
                                <ProjectCard project={p}></ProjectCard>
                            </>)
                        }
                    </div>
                }

                {
                    projects.length <= 0 &&
                    <p>There are currently no projects satisfying the criteria.</p>
                }
            </div>

            { /* discovery refinement sidebar */}
            {
                discoveryRefinementSidebarVisibility &&
                <div className='fixed left-0 top-0'>
                    <DiscoveryParametersSidebar
                        keyword={keyword}
                        sort={sort}
                        hat={hat}
                        hats={ownHats}
                        onModalClosed={hideDiscoveryParametersSidebar}
                        onDiscoveryParametersChanged={updateDiscoveryParameters}>
                    </DiscoveryParametersSidebar>
                </div>
            }
        </SingleColumnLayout>
    )
}

export default Discover