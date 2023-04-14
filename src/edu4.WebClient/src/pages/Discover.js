import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import RefineButton from '../comps/discover/RefineButton';
import SearchFilter from '../comps/discover/SearchFilter';
import SearchFilters from '../comps/discover/SearchFilters';
import { useState, useEffect } from 'react';
import RecommendedProjectCard from '../comps/discover/RecommendedProjectCard';
import SearchRefinements from '../comps/discover/SearchRefinements';
import { discover } from '../services/ProjectsService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { useAuth0 } from '@auth0/auth0-react';
import { me } from '../services/UsersService'

const Discover = () => {
    const [projects, setProjects] = useState([
    ]);

    const [hats, setHats] = useState([]);
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [searchRefinementsVisibility, setSearchRefinementsVisibility] = useState(false);

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
                    setHats(result.payload.hats);
                } else {
                    console.log("error fetching users hats");
                }
            } catch (ex) {
                console.log(ex);
            }
        })();
    }, [])


    function onSearchRefinementsChanged() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await discover(keyword, sort, hat != undefined ? hat.type : undefined, token);

                if (result.outcome === successResult) {
                    console.log("dohvaceni projekti");
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
                    console.log("nesto je do mreze");
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
        onSearchRefinementsChanged();
    }, [keyword, sort, hat])


    function updateProjectDiscoveryParameters(keyword, sort, hat) {
        setKeyword(keyword);
        setHat(hat);
        setSort(sort);
    }

    function showSearchRefinements() {
        setSearchRefinementsVisibility(true);
    }

    function hideSearchRefinements() {
        setSearchRefinementsVisibility(false);
    }

    return (
        <SingleColumnLayout
            title="Discover projects"
            description="Something encouraging here">

            <div className='mt-8 flex flex-col'>
                <RefineButton onClick={showSearchRefinements}></RefineButton>

                {/* filters */}
                <SearchFilters>
                    {
                        keyword != undefined &&
                        <SearchFilter
                            value={`keyword: ${keyword}`}
                            onRemoved={() => setKeyword(undefined)}>
                        </SearchFilter>
                    }
                    {
                        sort != undefined &&
                        <SearchFilter
                            value={`sort: ${sort}`}
                            onRemoved={() => setSort(undefined)}>
                        </SearchFilter>
                    }
                    {
                        hat != undefined &&
                        <SearchFilter
                            value={`fit for: my ${hat.type.toLowerCase()} hat`}
                            onRemoved={() => setHat(undefined)}>
                        </SearchFilter>
                    }
                </SearchFilters>
            </div>

            {
                projects.length > 0 &&
                <div className='mt-16 flex flex-col space-y-8'>
                    {
                        projects.map(p => <>
                            <div className=''>
                                <RecommendedProjectCard project={p}></RecommendedProjectCard>
                            </div>
                        </>)
                    }
                </div>
            }

            {
                projects.length <= 0 &&
                <div className='mt-16'>
                    <p>There are currently no projects satisfying the criteria.</p>
                </div>
            }

            {
                searchRefinementsVisibility &&
                <div className='fixed left-0 top-0'>
                    <SearchRefinements
                        keyword={keyword}
                        sort={sort}
                        hat={hat}
                        hats={hats}
                        onModalClosed={hideSearchRefinements}
                        onSearchRefinementsChanged={updateProjectDiscoveryParameters}>
                    </SearchRefinements>
                </div>
            }
        </SingleColumnLayout>
    )
}

export default Discover